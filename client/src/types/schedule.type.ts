export interface ScheduleStudentDto {
  id: number;
  firstName: string;
  lastName: string;
  username: string;
  balance: number;
}

export interface ScheduleSlotDto {
  id: number;
  dayOfWeek: number; 
  startTime: string; 
  capacity: number;
  students: ScheduleStudentDto[];
}

export interface CreateScheduleSlotRequest {
  dayOfWeek: number;
  startTime: string;
}

export interface MyScheduleSlotDto {
  id: number;
  dayOfWeek: number;
  startTime: string;
}