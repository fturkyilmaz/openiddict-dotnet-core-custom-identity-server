"use client";

import { create } from "zustand";
import { authApi, User } from "@/lib/api";
import { setSecureItem, getSecureItem, removeSecureItem, AUTH_STORAGE_KEYS } from "@/lib/secure-storage.client";

interface AuthState {
  user: User | null;
  token: string | null;
  isLoading: boolean;
  isAuthenticated: boolean;
}

interface AuthActions {
  setUser: (user: User | null) => void;
  setToken: (token: string | null) => void;
  login: (username: string, password: string) => Promise<void>;
  logout: () => void;
  initialize: () => void;
}

type AuthStore = AuthState & AuthActions;

export const useAuthStore = create<AuthStore>((set, get) => ({
  // State
  user: null,
  token: null,
  isLoading: true,
  isAuthenticated: false,

  // Actions
  setUser: (user: User | null) => {
    set({ user, isAuthenticated: !!user });
    // Persist to secure storage
    if (user) {
      setSecureItem(AUTH_STORAGE_KEYS.USER, user);
    } else {
      removeSecureItem(AUTH_STORAGE_KEYS.USER);
    }
  },

  setToken: (token: string | null) => {
    set({ token });
    // Persist token to secure storage
    if (token) {
      setSecureItem(AUTH_STORAGE_KEYS.TOKEN, { value: token });
    } else {
      removeSecureItem(AUTH_STORAGE_KEYS.TOKEN);
    }
  },

  login: async (username: string, password: string) => {
    set({ isLoading: true });

    try {
      // Login to get token
      const authResponse = await authApi.login(username, password);
      const token = authResponse.access_token;

      // Fetch user info from /auth/me
      const user = await authApi.getCurrentUser();

      // Update state
      set({
        token,
        user,
        isAuthenticated: true,
        isLoading: false,
      });

      // Persist to secure storage
      setSecureItem(AUTH_STORAGE_KEYS.TOKEN, { value: token });
      setSecureItem(AUTH_STORAGE_KEYS.USER, user);
    } catch (error) {
      set({ isLoading: false });
      throw error;
    }
  },

  logout: () => {
    authApi.logout();
    // Clear state
    set({
      user: null,
      token: null,
      isAuthenticated: false,
      isLoading: false,
    });
    // Clear secure storage
    removeSecureItem(AUTH_STORAGE_KEYS.USER);
    removeSecureItem(AUTH_STORAGE_KEYS.TOKEN);
  },

  initialize: () => {
    if (typeof window === "undefined") {
      set({ isLoading: false });
      return;
    }

    try {
      // Load from secure storage
      const storedUser = getSecureItem<User>(AUTH_STORAGE_KEYS.USER);
      const storedToken = getSecureItem<{ value: string }>(AUTH_STORAGE_KEYS.TOKEN);

      if (storedUser && storedToken) {
        set({
          user: storedUser,
          token: storedToken.value,
          isAuthenticated: true,
          isLoading: false,
        });
      } else {
        set({ isLoading: false });
      }
    } catch {
      set({ isLoading: false });
    }
  },
}));
