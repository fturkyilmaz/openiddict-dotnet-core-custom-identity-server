"use client";

import { createContext, useContext, useEffect, ReactNode } from "react";
import { useAuthStore } from "@/stores/auth/auth-store";
import { User } from "@/lib/api";

interface AuthContextType {
  user: User | null;
  isLoading: boolean;
  login: (username: string, password: string) => Promise<void>;
  register: (username: string, email: string, password: string, displayName?: string) => Promise<void>;
  logout: () => void;
  isAuthenticated: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const { user, isLoading, isAuthenticated, login: zustandLogin, logout: zustandLogout, initialize } = useAuthStore();

  useEffect(() => {
    // Initialize auth state from secure storage on mount
    initialize();
  }, [initialize]);

  const login = async (username: string, password: string) => {
    await zustandLogin(username, password);
  };

  const register = async (username: string, email: string, password: string, displayName?: string) => {
    const { authApi } = await import("@/lib/api");
    await authApi.register(username, email, password, displayName);
    // After registration, login automatically
    await zustandLogin(username, password);
  };

  const logout = () => {
    zustandLogout();
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        isLoading,
        login,
        register,
        logout,
        isAuthenticated,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
}
