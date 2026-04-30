<script setup lang="ts">
import { RouterView, RouterLink, useRouter } from 'vue-router';
import Button from 'primevue/button';
import Menu from 'primevue/menu';
import { useAuthStore } from '@/stores/auth';

const auth = useAuthStore();
const router = useRouter();

const navItems = [
    { label: 'Users', icon: 'pi pi-users', to: '/users' },
    { label: 'Clients', icon: 'pi pi-key', to: '/clients' },
];

function logout(): void {
    auth.logout();
    router.push({ name: 'login' });
}
</script>

<template>
    <div class="admin-shell">
        <aside class="admin-side">
            <div class="brand">Smolla Identity</div>
            <Menu :model="navItems">
                <template #item="{ item, props }">
                    <RouterLink v-if="item.to" :to="item.to" custom v-slot="{ navigate, isActive }">
                        <a v-bind="props.action" :class="{ active: isActive }" @click="navigate">
                            <span :class="item.icon" />
                            <span>{{ item.label }}</span>
                        </a>
                    </RouterLink>
                </template>
            </Menu>
            <Button label="Sign out" icon="pi pi-sign-out" severity="secondary" text @click="logout" />
        </aside>
        <main class="admin-main">
            <RouterView />
        </main>
    </div>
</template>

<style scoped>
.admin-shell {
    display: grid;
    grid-template-columns: 240px 1fr;
    height: 100%;
}

.admin-side {
    background: #f4f4f5;
    padding: 1rem;
    display: flex;
    flex-direction: column;
    gap: 1rem;
    border-right: 1px solid #e4e4e7;
}

.brand {
    font-weight: 600;
    font-size: 1.1rem;
}

.admin-main {
    padding: 2rem;
    overflow-y: auto;
}

a.active {
    font-weight: 600;
}
</style>
