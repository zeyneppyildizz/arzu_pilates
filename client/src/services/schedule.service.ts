import api from './api';
import type {
  CreateScheduleSlotRequest,
  ScheduleSlotDto,
  ScheduleStudentDto,
  MyScheduleSlotDto
} from '../types/schedule.type';

const controller = '/schedule';

export const scheduleService = {
  async getSchedule(): Promise<ScheduleSlotDto[]> {
    const response = await api.get<ScheduleSlotDto[]>(controller);
    return response.data;
  },

  async createSlot(data: CreateScheduleSlotRequest): Promise<ScheduleSlotDto> {
    const response = await api.post<ScheduleSlotDto>(`${controller}/slots`, data);
    return response.data;
  },

  async deleteSlot(slotId: number): Promise<void> {
    await api.delete(`${controller}/slots/${slotId}`);
  },

  async addStudentToSlot(slotId: number, studentId: number): Promise<ScheduleStudentDto> {
    const response = await api.post<ScheduleStudentDto>(
      `${controller}/slots/${slotId}/students`,
      { studentId }
    );
    return response.data;
  },

  async removeStudentFromSlot(slotId: number, studentId: number): Promise<void> {
    await api.delete(`${controller}/slots/${slotId}/students/${studentId}`);
  },

  async getMySchedule(): Promise<MyScheduleSlotDto[]> {
    const response = await api.get<MyScheduleSlotDto[]>(`${controller}/my`);
    return response.data;
  },
};