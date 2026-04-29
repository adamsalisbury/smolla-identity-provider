import { api } from '@/services/api';

export type OAuthClientType = 'Public' | 'Confidential';

export interface OAuthClient {
    id: string;
    clientId: string;
    displayName: string;
    clientType: OAuthClientType;
    redirectUris: string[];
    postLogoutRedirectUris: string[];
    permissions: string[];
    requirePkce: boolean;
    requireConsent: boolean;
}

export async function listClients(): Promise<OAuthClient[]> {
    const response = await api.get<OAuthClient[]>('/api/clients');
    return response.data;
}

export async function getClient(clientId: string): Promise<OAuthClient> {
    const response = await api.get<OAuthClient>(`/api/clients/${clientId}`);
    return response.data;
}
