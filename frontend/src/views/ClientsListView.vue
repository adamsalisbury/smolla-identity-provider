<script setup lang="ts">
import { onMounted, ref } from 'vue';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import Tag from 'primevue/tag';
import { listClients, type OAuthClient } from '@/services/clients';

const clients = ref<OAuthClient[]>([]);
const loading = ref(false);

async function load(): Promise<void> {
    loading.value = true;
    try {
        clients.value = await listClients();
    } finally {
        loading.value = false;
    }
}

onMounted(load);
</script>

<template>
    <h1>OAuth clients</h1>
    <DataTable :value="clients" :loading="loading" data-key="id" striped-rows>
        <Column field="clientId" header="Client ID" sortable />
        <Column field="displayName" header="Display name" sortable />
        <Column header="Type">
            <template #body="{ data }">
                <Tag :severity="(data as OAuthClient).clientType === 'Confidential' ? 'info' : 'warning'" :value="(data as OAuthClient).clientType" />
            </template>
        </Column>
        <Column header="PKCE">
            <template #body="{ data }">
                <i v-if="(data as OAuthClient).requirePkce" class="pi pi-check" />
                <i v-else class="pi pi-times" />
            </template>
        </Column>
    </DataTable>
</template>
