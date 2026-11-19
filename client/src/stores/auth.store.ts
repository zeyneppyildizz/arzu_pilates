import { defineStore } from "pinia";
import { ref } from "vue";
import { authService } from "../services/auth.service";
import type {
  LoginRequest,
  AuthResponse,
  RegisterRequest,
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
  };
});
