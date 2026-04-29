<script setup lang="ts">
import { onMounted, ref } from 'vue';
import Card from 'primevue/card';
import Button from 'primevue/button';
import Tag from 'primevue/tag';
import { getUser, disableUser, type User } from '@/services/users';

const props = defineProps<{ id: string }>();

const user = ref<User | null>(null);
const loading = ref(false);

async function load(): Promise<void> {
    loading.value = true;
    try {
        user.value = await getUser(props.id);
    } finally {
        loading.value = false;
    }
}

async function disable(): Promise<void> {
    if (!user.value) {
        return;
    }
    user.value = await disableUser(user.value.id);
}

onMounted(load);
</script>

<template>
    <div v-if="user">
        <h1>{{ user.displayName }}</h1>
        <Card>
            <template #content>
                <p><strong>Email:</strong> {{ user.email }}</p>
                <p><strong>Created:</strong> {{ new Date(user.createdAt).toLocaleString('en-GB') }}</p>
                <p>
                    <strong>Status:</strong>
                    <Tag :severity="user.isDisabled ? 'danger' : 'success'" :value="user.isDisabled ? 'Disabled' : 'Active'" />
                </p>
                <p>
                    <strong>Roles:</strong>
                    <Tag v-for="role in user.roles" :key="role" :value="role" class="role-tag" />
                </p>
                <Button v-if="!user.isDisabled" label="Disable user" severity="danger" @click="disable" />
            </template>
        </Card>
    </div>
    <div v-else-if="loading">Loading…</div>
</template>

<style scoped>
.role-tag {
    margin-right: 0.25rem;
}
</style>
