<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';
import ProgressSpinner from 'primevue/progressspinner';
import { useAuthStore } from '@/stores/auth';

const auth = useAuthStore();
const router = useRouter();
const error = ref<string | null>(null);

onMounted(async () => {
    try {
        const returnTo = await auth.handleCallback();
        await router.replace(returnTo ?? '/');
    } catch (err) {
        error.value = err instanceof Error ? err.message : 'Sign-in failed.';
    }
});
</script>

<template>
    <div class="callback-shell">
        <ProgressSpinner v-if="!error" />
        <p v-else class="error">{{ error }}</p>
    </div>
</template>

<style scoped>
.callback-shell {
    display: grid;
    place-items: center;
    height: 100vh;
}
.error {
    color: #b91c1c;
}
</style>
