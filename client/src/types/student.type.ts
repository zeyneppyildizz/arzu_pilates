export interface StudentDto {
    id: number;
    firstName: string;
    lastName: string;
    username: string;
    balance: number;
    hasAnyPayment: boolean;
}

export interface CreateStudentRequest {
    firstName: string;
    lastName: string;
    username: string;
    password: string; 
    balance: number;
    amount: number; 
}
export interface AddPaymentRequest {
    studentId: number;
    addedSessions: number;
    amount: number;
}