export interface LoginRequest {
    username: string;
    password: string;
}

export interface AuthResponse {
    id: number;
    firstName: string;
    lastName: string;
    username: string;
    password: string;
    role: number; 
    balance?: number; 
    token?: string;
    mustChangePassword?: boolean;
}

export interface RegisterRequest {
    firstName: string;
    lastName: string;
    username: string;
    password: string;
}

export interface ChangePasswordRequest {
  currentPassword?: string;
  newPassword: string;
}

export interface ResetPasswordResponse {
  message: string;
  tempPassword: string;
  username: string;
  studentName: string;
}

export interface UpdateProfileRequest {
  firstName: string;
  lastName: string;
  username: string;
}

export interface UpdateProfileResponse {
  message: string;
  id: number;
  firstName: string;
  lastName: string;
  username: string;
}