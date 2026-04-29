import axios, { type AxiosInstance } from 'axios';
import { useAuthStore } from '@/stores/auth';

const baseURL = import.meta.env.VITE_API_BASE_URL.replace(/\/$/, '');

export const api: AxiosInstance = axios.create({ baseURL });

api.interceptors.request.use((config) => {
    const auth = useAuthStore();
    if (auth.accessToken) {
        config.headers.set('Authorization', `Bearer ${auth.accessToken}`);
    }
    return config;
});

api.interceptors.response.use(
    (response) => response,
    (error: unknown) => {
        if (axios.isAxiosError(error) && error.response?.status === 401) {
            const auth = useAuthStore();
            auth.logout();
        }
        return Promise.reject(error instanceof Error ? error : new Error(String(error)));
    },
);
