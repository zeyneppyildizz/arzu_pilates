import api from "./api";
import type {
  StudentDto,
  CreateStudentRequest,
  AddPaymentRequest,
} from "../types/student.type";

const controller = "/users";

export const studentService = {
  async getAllStudents(): Promise<StudentDto[]> {
    const response = await api.get<StudentDto[]>(`${controller}/students`);
    return response.data;
  },

  async addStudent(data: CreateStudentRequest): Promise<{ message: string }> {
    const response = await api.post(`${controller}/add-student`, data);
    return response.data;
  },

  async deleteStudent(studentId: number): Promise<void> {
    await api.delete(`${controller}/${studentId}`);
  },

  async addPayment(
    data: AddPaymentRequest,
  ): Promise<{ message: string; newBalance: number }> {
    const response = await api.post<{ message: string; newBalance: number }>(
      "/payments/add",
      data,
    );
    return response.data;
  },

  async getMyProfile(): Promise<StudentDto> {
    const response = await api.get<StudentDto>(`${controller}/me`);
    return response.data;
  },
};
