import { defineStore } from 'pinia';
import { ref } from 'vue';
import { attendanceService } from '../services/attendance.service';
import type { ScanQrResponse } from '../types/attendance.type';

export const useAttendanceStore = defineStore('attendance', () => {
  const isLoading = ref(false);
  const error = ref<string>('');
  const todayAttendanceCount = ref<number | null>(null);
  const isLoadingTodayCount = ref(false);
  const todayAttendanceError = ref('');

  const fetchTodayCount = async () => {
    isLoadingTodayCount.value = true;
    todayAttendanceError.value = '';

    try {
      todayAttendanceCount.value = await attendanceService.getTodayCount();
    } catch (err: any) {
      todayAttendanceError.value = err.response?.data?.message || 'Bugünkü yoklama bilgisi alınamadı.';
    } finally {
      isLoadingTodayCount.value = false;
    }
  };

  const scanQrCode = async (token: string): Promise<ScanQrResponse> => {
    isLoading.value = true;
    error.value = ''; 
    
    try {
      const result = await attendanceService.scanQr(token);
      return result;
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Yoklama alınamadı! Lütfen tekrar deneyin.';
      throw err;
    } finally {
      isLoading.value = false;
    }
  };

  return {
    isLoading,
    error,
    todayAttendanceCount,
    isLoadingTodayCount,
    todayAttendanceError,
    fetchTodayCount,
    scanQrCode
  };
});