import { defineStore } from 'pinia';
import { ref } from 'vue';
import { scheduleService } from '../services/schedule.service';
import type { CreateScheduleSlotRequest, ScheduleSlotDto, MyScheduleSlotDto} from '../types/schedule.type';

export const useScheduleStore = defineStore('schedule', () => {
  const slots = ref<ScheduleSlotDto[]>([]);
  const mySlots = ref<MyScheduleSlotDto[]>([]);
  const isLoading = ref(false);
  const error = ref('');

  const sortSlots = () => {
    slots.value.sort((a, b) =>
      a.dayOfWeek !== b.dayOfWeek
        ? a.dayOfWeek - b.dayOfWeek
        : a.startTime.localeCompare(b.startTime)
    );
  };

  const fetchSchedule = async () => {
    isLoading.value = true;
    error.value = '';
    try {
      slots.value = await scheduleService.getSchedule();
      sortSlots();
      return true;
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Ders programı yüklenemedi.';
      return false;
    } finally {
      isLoading.value = false;
    }
  };

  const createSlot = async (payload: CreateScheduleSlotRequest) => {
    error.value = '';
    try {
      const created = await scheduleService.createSlot(payload);
      slots.value.push(created);
      sortSlots();
      return created;
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Ders saati eklenemedi.';
      return null;
    }
  };

  const deleteSlot = async (slotId: number) => {
    error.value = '';
    try {
      await scheduleService.deleteSlot(slotId);
      slots.value = slots.value.filter(s => s.id !== slotId);
      return true;
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Ders saati silinemedi.';
      return false;
    }
  };

  const addStudentToSlot = async (slotId: number, studentId: number) => {
    error.value = '';
    try {
      const addedStudent = await scheduleService.addStudentToSlot(slotId, studentId);
      const slot = slots.value.find(s => s.id === slotId);
      if (slot && !slot.students.some(s => s.id === addedStudent.id)) {
        slot.students.push(addedStudent);
      }
      return true;
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Öğrenci derse eklenemedi.';
      return false;
    }
  };

  const removeStudentFromSlot = async (slotId: number, studentId: number) => {
    error.value = '';
    try {
      await scheduleService.removeStudentFromSlot(slotId, studentId);
      const slot = slots.value.find(s => s.id === slotId);
      if (slot) {
        slot.students = slot.students.filter(s => s.id !== studentId);
      }
      return true;
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Öğrenci dersten çıkarılamadı.';
      return false;
    }
  };

  const fetchMySchedule = async () => {
    try {
      mySlots.value = await scheduleService.getMySchedule();
      return true;
    } catch {
      mySlots.value = [];
      return false;
    }
  };


  return {
    slots,
    isLoading,
    error,
    fetchSchedule,
    createSlot,
    deleteSlot,
    addStudentToSlot,
    removeStudentFromSlot,
    mySlots,
    fetchMySchedule
  };
});