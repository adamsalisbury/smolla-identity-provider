import { api } from '@/services/api';

export interface VersionResponse {
    version: string;
}

export async function getVersion(): Promise<string> {
    const response = await api.get<VersionResponse>('/api/version');
    return response.data.version;
}
