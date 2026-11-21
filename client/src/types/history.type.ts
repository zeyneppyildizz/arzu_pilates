import type { AttendanceType } from "./attendance.type";

export interface AttendanceDto {
  id: number;
  date: string;
  balanceAfter: number;
  type: AttendanceType;
  note?: string | null;
}

export interface PaymentDto {
  id: number;
  date: string;
  addedSessions: number;
  amount: number;
}

export interface StudentHistoryResponse {
  attendances: AttendanceDto[];
  payments: PaymentDto[];
}

export type HistoryActivity =
  | {
      key: string;
      type: "attendance";
      date: string;
      balanceAfter: number;
      attendanceType: AttendanceType;
      note?: string | null;
    }
  | {
      key: string;
      type: "payment";
      date: string;
      addedSessions: number;
      amount: number;
    };
