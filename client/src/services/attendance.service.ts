import api from './api';
import type { ScanQrResponse } from '../types/attendance.type'; 

export const attendanceService = {
  async getTodayCount(): Promise<number> {
    const response = await api.get<{ count: number }>('/attendance/today-summary');
    return response.data.count;
  },

  async scanQr(token: string): Promise<ScanQrResponse> {
    const response = await api.post<ScanQrResponse>('/attendance/scan', { token });
    return response.data;
  }
};