import api from './api';
import type { LoginRequest, AuthResponse, RegisterRequest, ChangePasswordRequest,
    ResetPasswordResponse, UpdateProfileRequest, UpdateProfileResponse } from '../types/auth.type'; 

const controller = '/users'; 

export const authService = {
    async login(data: LoginRequest): Promise<AuthResponse> {
        const response = await api.post<AuthResponse>(`${controller}/login`, data);
        return response.data;
    },

    async register(userData: RegisterRequest): Promise<{ message: string; studentId: number }> {
        const response = await api.post(`${controller}/create-student`, userData);
        return response.data;
    },

    async changePassword(data: ChangePasswordRequest): Promise<{ message: string }> {
        const response = await api.post<{ message: string }>(`${controller}/change-password`, data);
        return response.data;
    },

    async resetStudentPassword(studentId: number): Promise<ResetPasswordResponse> {
        const response = await api.post<ResetPasswordResponse>(`${controller}/${studentId}/reset-password`);
        return response.data;
    },

    async getMyProfile(): Promise<AuthResponse> {
        const response = await api.get<AuthResponse>(`${controller}/me`);
        return response.data;
    },

    async updateProfile(data: UpdateProfileRequest): Promise<UpdateProfileResponse> {
        const response = await api.put<UpdateProfileResponse>(`${controller}/me`, data);
        return response.data;
    },
};