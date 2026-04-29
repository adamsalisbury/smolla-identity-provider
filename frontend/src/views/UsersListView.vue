<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import InputText from 'primevue/inputtext';
import Button from 'primevue/button';
import Tag from 'primevue/tag';
import { searchUsers, type User } from '@/services/users';

const router = useRouter();
const users = ref<User[]>([]);
const loading = ref(false);
const searchTerm = ref('');

async function load(): Promise<void> {
    loading.value = true;
    try {
        users.value = await searchUsers({ search: searchTerm.value || undefined });
    } finally {
        loading.value = false;
    }
}

onMounted(load);

function open(user: User): void {
    router.push({ name: 'user-detail', params: { id: user.id } });
}
</script>

<template>
    <header class="users-header">
        <h1>Users</h1>
        <div class="users-actions">
            <InputText v-model="searchTerm" placeholder="Search…" @keyup.enter="load" />
            <Button label="Search" icon="pi pi-search" @click="load" />
        </div>
    </header>

    <DataTable :value="users" :loading="loading" data-key="id" striped-rows row-hover @row-click="(e) => open(e.data as User)">
        <Column field="email" header="Email" sortable />
        <Column field="displayName" header="Display name" sortable />
        <Column header="Roles">
            <template #body="{ data }">
                <Tag v-for="role in (data as User).roles" :key="role" :value="role" class="role-tag" />
            </template>
        </Column>
        <Column header="Status">
            <template #body="{ data }">
                <Tag :severity="(data as User).isDisabled ? 'danger' : 'success'" :value="(data as User).isDisabled ? 'Disabled' : 'Active'" />
            </template>
        </Column>
    </DataTable>
</template>

<style scoped>
.users-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 1.5rem;
}
.users-actions {
    display: flex;
    gap: 0.5rem;
}
.role-tag {
    margin-right: 0.25rem;
}
</style>
