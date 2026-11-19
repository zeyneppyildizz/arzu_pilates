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
}

export interface RegisterRequest {
    firstName: string;
    lastName: string;
    username: string;
    password: string;
}