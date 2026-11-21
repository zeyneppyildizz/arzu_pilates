import { defineStore } from "pinia";
import { ref } from "vue";
import { authService } from "../services/auth.service";
import type {
  LoginRequest,
  AuthResponse,
  RegisterRequest,
  ChangePasswordRequest,
  UpdateProfileRequest,
} from "../types/auth.type";

export const useAuthStore = defineStore("auth", () => {
  const getStoredUser = (): AuthResponse | null => {
    try {
      const raw = localStorage.getItem("user");
      return raw ? JSON.parse(raw) : null;
    } catch {
      localStorage.removeItem("user");
      return null;
    }
  };

  const user = ref<AuthResponse | null>(getStoredUser());

  const error = ref<string>("");
  const isLoading = ref<boolean>(false);

  const login = async (credentials: LoginRequest) => {
    isLoading.value = true;
    try {
      error.value = "";
      const userData = await authService.login(credentials);

      user.value = userData;

      localStorage.setItem("user", JSON.stringify(userData));

      return true;
    } catch (err: any) {
      error.value = err.response?.data?.message || "Giriş başarısız";
      return false;
    } finally {
      isLoading.value = false;
    }
  };

  const register = async (userData: RegisterRequest) => {
    isLoading.value = true;
    try {
      error.value = "";
      await authService.register(userData);
      return true;
    } catch (err: any) {
      error.value =
        err.response?.data?.message || "Kayıt işlemi başarısız oldu.";
      return false;
    } finally {
      isLoading.value = false;
    }
  };

  const changePassword = async (payload: ChangePasswordRequest) => {
    isLoading.value = true;
    try {
      error.value = "";
      await authService.changePassword(payload);

      if (user.value) {
        user.value = {
          ...user.value,
          mustChangePassword: false,
        };
        localStorage.setItem("user", JSON.stringify(user.value));
      }

      return true;
    } catch (err: any) {
      error.value = err.response?.data?.message || "Şifre güncellenemedi.";
      return false;
    } finally {
      isLoading.value = false;
    }
  };

  const fetchMyProfile = async () => {
    try {
      const profile = await authService.getMyProfile();
      if (user.value) {
        user.value = {
          ...user.value,
          firstName: profile.firstName,
          lastName: profile.lastName,
          username: profile.username,
          balance: profile.balance,
          mustChangePassword: profile.mustChangePassword,
        };
        localStorage.setItem("user", JSON.stringify(user.value));
      }
      return true;
    } catch {
      return false;
    }
  };

  const updateProfile = async (payload: UpdateProfileRequest) => {
    try {
      error.value = "";
      const updated = await authService.updateProfile(payload);

      if (user.value) {
        user.value = {
          ...user.value,
          firstName: updated.firstName,
          lastName: updated.lastName,
          username: updated.username,
        };
        localStorage.setItem("user", JSON.stringify(user.value));
      }

      return true;
    } catch (err: any) {
      error.value = err.response?.data?.message || "Bilgiler güncellenemedi.";
      return false;
    }
  };

  const resetStudentPassword = async (studentId: number) => {
  error.value = ''
  try {
    return await authService.resetStudentPassword(studentId)
  } catch (err: any) {
    error.value = err.response?.data?.message || 'Şifre sıfırlanırken bir hata oluştu.'
    return null
  }
}

  const logout = () => {
    user.value = null;
    error.value = "";
    localStorage.removeItem("user");
  };

  return {
    user,
    error,
    loading: isLoading,
    login,
    register,
    logout,
    changePassword,
    fetchMyProfile,
    updateProfile,
    resetStudentPassword,
  };
});
