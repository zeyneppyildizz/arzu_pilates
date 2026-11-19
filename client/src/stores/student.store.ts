import { defineStore } from "pinia";
import { ref } from "vue";
import { studentService } from "../services/student.service";
import type { StudentDto, AddPaymentRequest } from "../types/student.type";

export const useStudentStore = defineStore("student", () => {
  const students = ref<StudentDto[]>([]);

  const isLoading = ref(false);
  const error = ref<string>("");

  const fetchStudents = async () => {
    isLoading.value = true;
    try {
      error.value = "";
      students.value = await studentService.getAllStudents();
    } catch (err: any) {
      error.value = err.response?.data?.message || "Öğrenciler yüklenemedi";
    } finally {
      isLoading.value = false;
    }
  };

  const deleteStudent = async (studentId: number) => {
    isLoading.value = true;
    try {
      error.value = "";
      await studentService.deleteStudent(studentId);
      students.value = students.value.filter((s) => s.id !== studentId);
    } catch (err: any) {
      error.value =
        err.response?.data?.message || "Silme işlemi başarısız oldu.";
    } finally {
      isLoading.value = false;
    }
  };

  const addPayment = async (paymentData: AddPaymentRequest) => {
    isLoading.value = true;
    try {
      error.value = "";
      const result = await studentService.addPayment(paymentData);
      const student = students.value.find(
        (s) => s.id === paymentData.studentId,
      );
      if (student) {
        student.balance = result.newBalance;
        student.hasAnyPayment = true;
      }
    } catch (err: any) {
      error.value = err.response?.data?.message || "Paket yüklenemedi.";
    } finally {
      isLoading.value = false;
    }
  };

  const fetchMyProfile = async (): Promise<StudentDto | null> => {
    try {
      error.value = "";
      return await studentService.getMyProfile();
    } catch (err: any) {
      error.value =
        err.response?.data?.message || "Profil bilgileri alınamadı.";
      return null;
    }
  };

  return {
    students,
    isLoading,
    error,
    fetchStudents,
    deleteStudent,
    addPayment,
    fetchMyProfile,
  };
});
