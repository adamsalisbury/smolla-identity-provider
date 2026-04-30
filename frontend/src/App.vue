<script setup lang="ts">
import { RouterView } from 'vue-router';
import AppFooter from '@/components/AppFooter.vue';
</script>

<template>
    <div class="app-shell">
        <RouterView class="app-content" />
        <AppFooter />
    </div>
</template>

<style>
:root {
    font-family: system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
}

body,
html,
#app {
    margin: 0;
    padding: 0;
    height: 100%;
}

.app-shell {
    display: flex;
    flex-direction: column;
    min-height: 100vh;
}

.app-content {
    flex: 1 1 auto;
    min-height: 0;
}
</style>
