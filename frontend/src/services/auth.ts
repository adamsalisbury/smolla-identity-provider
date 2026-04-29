/**
 * Browser-side OAuth 2.0 Authorisation Code + PKCE flow against the
 * Smolla identity provider. Tokens are stored in sessionStorage so they
 * are scoped to the tab and lost on close.
 */

const STORAGE_KEY = 'smolla.identity.tokens';
const PKCE_KEY = 'smolla.identity.pkce';

export interface TokenSet {
    accessToken: string;
    idToken: string | null;
    refreshToken: string | null;
    expiresAtMs: number;
    tokenType: string;
}

interface PendingAuth {
    codeVerifier: string;
    state: string;
    returnTo: string | null;
}

interface AuthorisationResult {
    tokens: TokenSet;
    returnTo: string | null;
}

function authority(): string {
    return import.meta.env.VITE_OIDC_AUTHORITY.replace(/\/$/, '');
}

function clientId(): string {
    return import.meta.env.VITE_OIDC_CLIENT_ID;
}

function redirectUri(): string {
    return import.meta.env.VITE_OIDC_REDIRECT_URI;
}

export function getStoredTokens(): TokenSet | null {
    const raw = sessionStorage.getItem(STORAGE_KEY);
    if (!raw) {
        return null;
    }
    try {
        return JSON.parse(raw) as TokenSet;
    } catch {
        return null;
    }
}

function storeTokens(tokens: TokenSet): void {
    sessionStorage.setItem(STORAGE_KEY, JSON.stringify(tokens));
}

export function clearTokens(): void {
    sessionStorage.removeItem(STORAGE_KEY);
}

function base64UrlEncode(bytes: Uint8Array): string {
    let binary = '';
    bytes.forEach((b) => {
        binary += String.fromCharCode(b);
    });
    return btoa(binary).replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
}

async function generatePkce(): Promise<{ verifier: string; challenge: string }> {
    const verifierBytes = new Uint8Array(32);
    crypto.getRandomValues(verifierBytes);
    const verifier = base64UrlEncode(verifierBytes);

    const digest = await crypto.subtle.digest('SHA-256', new TextEncoder().encode(verifier));
    const challenge = base64UrlEncode(new Uint8Array(digest));

    return { verifier, challenge };
}

function generateState(): string {
    const bytes = new Uint8Array(16);
    crypto.getRandomValues(bytes);
    return base64UrlEncode(bytes);
}

export async function beginAuthorisation(returnTo: string | null = null): Promise<void> {
    const { verifier, challenge } = await generatePkce();
    const state = generateState();

    const pending: PendingAuth = { codeVerifier: verifier, state, returnTo };
    sessionStorage.setItem(PKCE_KEY, JSON.stringify(pending));

    const url = new URL(`${authority()}/connect/authorize`);
    url.searchParams.set('response_type', 'code');
    url.searchParams.set('client_id', clientId());
    url.searchParams.set('redirect_uri', redirectUri());
    url.searchParams.set('scope', 'openid profile email roles offline_access');
    url.searchParams.set('state', state);
    url.searchParams.set('code_challenge', challenge);
    url.searchParams.set('code_challenge_method', 'S256');

    window.location.assign(url.toString());
}

export async function completeAuthorisation(): Promise<AuthorisationResult> {
    const params = new URLSearchParams(window.location.search);
    const code = params.get('code');
    const state = params.get('state');

    if (!code || !state) {
        throw new Error('Authorisation response is missing required parameters.');
    }

    const rawPending = sessionStorage.getItem(PKCE_KEY);
    if (!rawPending) {
        throw new Error('No PKCE state found for this callback.');
    }
    const pending = JSON.parse(rawPending) as PendingAuth;
    sessionStorage.removeItem(PKCE_KEY);

    if (pending.state !== state) {
        throw new Error('OAuth state mismatch — possible CSRF.');
    }

    const body = new URLSearchParams();
    body.set('grant_type', 'authorization_code');
    body.set('client_id', clientId());
    body.set('code', code);
    body.set('redirect_uri', redirectUri());
    body.set('code_verifier', pending.codeVerifier);

    const response = await fetch(`${authority()}/connect/token`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body,
    });

    if (!response.ok) {
        throw new Error(`Token exchange failed: ${response.status}`);
    }

    const json = (await response.json()) as {
        access_token: string;
        id_token?: string;
        refresh_token?: string;
        expires_in: number;
        token_type: string;
    };

    const tokens: TokenSet = {
        accessToken: json.access_token,
        idToken: json.id_token ?? null,
        refreshToken: json.refresh_token ?? null,
        expiresAtMs: Date.now() + json.expires_in * 1000,
        tokenType: json.token_type,
    };

    storeTokens(tokens);
    return { tokens, returnTo: pending.returnTo };
}
