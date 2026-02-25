import axios from "axios";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";
const TOKEN_COOKIE_NAME = "access_token";

// Create axios instance with interceptors
const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

// Request interceptor to add auth token
api.interceptors.request.use(
  (config) => {
    if (typeof window !== "undefined") {
      const token = localStorage.getItem("token");
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
    }
    return config;
  },
  (error) => Promise.reject(error),
);

// Response interceptor to handle auth errors
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      if (typeof window !== "undefined") {
        localStorage.removeItem("token");
        localStorage.removeItem("user");
        // Optionally redirect to login
        window.location.href = "/auth/v2/login";
      }
    }
    return Promise.reject(error);
  },
);

export interface User {
  id: string;
  userName: string;
  email: string;
  emailVerified: boolean;
  twoFactorEnabled: boolean;
  displayName?: string;
  roles?: string[];
}

export interface AuthResponse {
  access_token: string;
  token_type: string;
  expires_in: number;
}

// Auth API
export const authApi = {
  async login(username: string, password: string): Promise<AuthResponse> {
    const params = new URLSearchParams();
    params.append("grant_type", "password");
    params.append("username", username);
    params.append("password", password);
    params.append("client_id", "shopping-admin");
    params.append("client_secret", "dev-secret");
    params.append("scope", "api openid profile offline_access");

    const response = await axios.post(`${API_BASE_URL}/connect/token`, params, {
      headers: {
        "Content-Type": "application/x-www-form-urlencoded",
      },
    });

    const { data } = response;

    // Store token securely
    if (typeof window !== "undefined") {
      localStorage.setItem("token", data.access_token);
      // Set cookie for server-side auth check (expires in 1 day)
      const expires = new Date();
      expires.setDate(expires.getDate() + 1);
      document.cookie = `${TOKEN_COOKIE_NAME}=${data.access_token};path=/;expires=${expires.toUTCString()};SameSite=Lax;Secure`;
    }

    return data;
  },

  async register(username: string, email: string, password: string, displayName?: string): Promise<any> {
    const response = await api.post("/auth/register", {
      userName: username,
      email,
      password,
      displayName,
    });
    return response.data;
  },

  async getCurrentUser(): Promise<User> {
    const response = await api.get("/auth/me");

    const data = response.data;

    // Store user info
    if (typeof window !== "undefined") {
      localStorage.setItem("user", JSON.stringify(data));
    }

    return data;
  },

  logout(): void {
    if (typeof window !== "undefined") {
      localStorage.removeItem("token");
      localStorage.removeItem("user");
      // Clear cookie
      document.cookie = `${TOKEN_COOKIE_NAME}=;path=/;expires=Thu, 01 Jan 1970 00:00:00 GMT`;
      // Redirect to login
      window.location.href = "/auth/v2/login";
    }
  },

  getStoredUser(): User | null {
    if (typeof window === "undefined") return null;
    const userStr = localStorage.getItem("user");
    return userStr ? JSON.parse(userStr) : null;
  },

  isAuthenticated(): boolean {
    if (typeof window === "undefined") return false;
    return !!localStorage.getItem("token");
  },
};

// User Management API
export const usersApi = {
  getAll: () => api.get("/api/v1.0/users"),
  search: (search?: string, status?: number) => {
    const params = new URLSearchParams();
    if (search) params.append("search", search);
    if (status !== undefined) params.append("status", status.toString());
    return api.get(`/api/v1.0/users?${params}`);
  },
};

// Role Management API
export const rolesApi = {
  getAll: () => api.get("/api/v1.0/user-roles"),
  assign: (userId: string, roleId: string) => api.post("/api/v1.0/user-roles", { userId, roleId }),
  remove: (userId: string, roleId: string) => api.delete("/api/v1.0/user-roles", { data: { userId, roleId } }),
};

// Clients API
export const clientsApi = {
  getAll: () => api.get("/api/v1.0/clients"),
};

export default api;
