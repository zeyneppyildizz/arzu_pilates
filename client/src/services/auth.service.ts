import api from './api';
import type { LoginRequest, AuthResponse, RegisterRequest } from '../types/auth.type'; 

const controller = '/users'; 

export const authService = {
    async login(data: LoginRequest): Promise<AuthResponse> {
        const response = await api.post<AuthResponse>(`${controller}/login`, data);
        return response.data;
    },

    async register(userData: RegisterRequest): Promise<{ message: string; studentId: number }> {
        const response = await api.post(`${controller}/create-student`, userData);
        return response.data;
    }
};