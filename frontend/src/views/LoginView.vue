<script setup lang="ts">
import { useRoute } from 'vue-router';
import Button from 'primevue/button';
import { useAuthStore } from '@/stores/auth';

const auth = useAuthStore();
const route = useRoute();

async function signIn(): Promise<void> {
    const returnTo = typeof route.query.returnTo === 'string' ? route.query.returnTo : null;
    await auth.login(returnTo ?? undefined);
}
</script>

<template>
    <div class="login-shell">
        <div class="login-card">
            <h1>Smolla Identity</h1>
            <p>Administrator sign-in.</p>
            <Button label="Sign in" icon="pi pi-sign-in" @click="signIn" />
        </div>
    </div>
</template>

<style scoped>
.login-shell {
    display: grid;
    place-items: center;
    height: 100vh;
    background: #fafafa;
}

.login-card {
    background: white;
    border: 1px solid #e4e4e7;
    border-radius: 8px;
    padding: 2rem;
    min-width: 320px;
    text-align: center;
    display: flex;
    flex-direction: column;
    gap: 1rem;
}
</style>
