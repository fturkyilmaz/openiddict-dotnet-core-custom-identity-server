"use client";

import { createContext, useContext, useState, useEffect, ReactNode } from "react";
import { authApi, User } from "@/lib/api";

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
    const [user, setUser] = useState<User | null>(null);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        // Check for existing session on mount
        const storedUser = authApi.getStoredUser();
        if (storedUser) {
            setUser(storedUser);
        }
        setIsLoading(false);
    }, []);

    const login = async (username: string, password: string) => {
        await authApi.login(username, password);
        const user = await authApi.getCurrentUser();
        setUser(user);
    };

    const register = async (
        username: string,
        email: string,
        password: string,
        displayName?: string
    ) => {
        await authApi.register(username, email, password, displayName);
        // After registration, login automatically
        await authApi.login(username, password);
        const user = await authApi.getCurrentUser();
        setUser(user);
    };

    const logout = () => {
        authApi.logout();
        setUser(null);
    };

    return (
        <AuthContext.Provider
            value={{
                user,
                isLoading,
                login,
                register,
                logout,
                isAuthenticated: !!user,
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
