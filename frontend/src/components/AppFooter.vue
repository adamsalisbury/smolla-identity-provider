<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { getVersion } from '@/services/version';

const version = ref<string | null>(null);

onMounted(async () => {
    try {
        version.value = await getVersion();
    } catch {
        version.value = null;
    }
});
</script>

<template>
    <footer class="app-footer">
        <span class="brand">Smolla Identity</span>
        <span v-if="version" class="version">v{{ version }}</span>
    </footer>
</template>

<style scoped>
.app-footer {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 0.5rem 1rem;
    font-size: 0.8rem;
    color: #71717a;
    background: #fafafa;
    border-top: 1px solid #e4e4e7;
}

.version {
    font-variant-numeric: tabular-nums;
}
</style>
