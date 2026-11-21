export interface ScanQrResponse {
    newBalance: number;
    message?: string;
}

export type AttendanceType = 0 | 1 | 2; 

export interface ManualDeductRequest {
  studentId: number;
  type: 1 | 2;
  note?: string;
}

export interface ManualDeductResponse {
  message: string;
  newBalance: number;
}