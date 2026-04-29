import { api } from '@/services/api';

export interface User {
    id: string;
    email: string;
    displayName: string;
    isDisabled: boolean;
    isEmailVerified: boolean;
    createdAt: string;
    lastLoginAt: string | null;
    roles: string[];
}

export interface UserSearchParams {
    search?: string;
    includeDisabled?: boolean;
    skip?: number;
    take?: number;
}

export async function searchUsers(params: UserSearchParams = {}): Promise<User[]> {
    const response = await api.get<User[]>('/api/users', { params });
    return response.data;
}

export async function getUser(id: string): Promise<User> {
    const response = await api.get<User>(`/api/users/${id}`);
    return response.data;
}

export interface CreateUserBody {
    email: string;
    password: string;
    displayName: string;
    roles?: string[];
}

export async function createUser(body: CreateUserBody): Promise<User> {
    const response = await api.post<User>('/api/users', body);
    return response.data;
}

export async function disableUser(id: string): Promise<User> {
    const response = await api.post<User>(`/api/users/${id}/disable`);
    return response.data;
}

export async function assignRoles(id: string, roles: string[]): Promise<User> {
    const response = await api.put<User>(`/api/users/${id}/roles`, roles);
    return response.data;
}
