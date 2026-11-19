import api from "./api";
import type { StudentHistoryResponse } from "../types/history.type";

const controller = "/users";
  
export const historyService = {
  async getStudentHistory(studentId: number): Promise<StudentHistoryResponse> {
    const response = await api.get<StudentHistoryResponse>(
      `${controller}/${studentId}/history`
    );
    return response.data;
  }
}