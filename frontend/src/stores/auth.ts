import { defineStore } from 'pinia';
import { computed, ref } from 'vue';
import { beginAuthorisation, completeAuthorisation, clearTokens, getStoredTokens } from '@/services/auth';
import type { TokenSet } from '@/services/auth';

export const useAuthStore = defineStore('auth', () => {
  const tokens = ref<TokenSet | null>(getStoredTokens());

  const isAuthenticated = computed(() => tokens.value !== null && Date.now() < tokens.value.expiresAtMs);

  const accessToken = computed(() => tokens.value?.accessToken ?? null);

  async function login(returnTo?: string): Promise<void> {
    await beginAuthorisation(returnTo);
  }

  async function handleCallback(): Promise<string | null> {
    const result = await completeAuthorisation();
    tokens.value = result.tokens;
    return result.returnTo;
  }

  function logout(): void {
    clearTokens();
    tokens.value = null;
  }

  return {
    tokens,
    isAuthenticated,
    accessToken,
    login,
    handleCallback,
    logout,
  };
});
