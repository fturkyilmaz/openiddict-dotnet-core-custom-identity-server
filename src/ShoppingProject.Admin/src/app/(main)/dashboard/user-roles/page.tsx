"use client";

import { useQuery } from "@tanstack/react-query";

import { rolesApi } from "@/lib/api";
import { UserRoleTable } from "@/components/user-roles/user-role-table";
import type { UserRole } from "@/components/user-roles/types";

export default function UserRolesPage() {
    const { data: userRoles = [], isLoading, error } = useQuery<UserRole[]>({
        queryKey: ["user-roles"],
        queryFn: async () => {
            const response = await rolesApi.getAll();
            return response.data;
        },
    });

    if (isLoading) {
        return (
            <div className="@container/main flex flex-col gap-4 md:gap-6">
                <div className="flex items-center justify-between">
                    <div>
                        <h1 className="text-2xl font-semibold tracking-tight">User Roles</h1>
                        <p className="text-muted-foreground text-sm">Manage user role assignments.</p>
                    </div>
                </div>
                <div className="flex items-center justify-center h-64">
                    <div className="text-muted-foreground">Loading...</div>
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="@container/main flex flex-col gap-4 md:gap-6">
                <div className="flex items-center justify-between">
                    <div>
                        <h1 className="text-2xl font-semibold tracking-tight">User Roles</h1>
                        <p className="text-muted-foreground text-sm">Manage user role assignments.</p>
                    </div>
                </div>
                <div className="flex items-center justify-center h-64">
                    <div className="text-red-500">Failed to load user roles. Please check your connection.</div>
                </div>
            </div>
        );
    }

    return (
        <div className="@container/main flex flex-col gap-4 md:gap-6">
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-2xl font-semibold tracking-tight">User Roles</h1>
                    <p className="text-muted-foreground text-sm">Manage user role assignments.</p>
                </div>
            </div>
            <UserRoleTable data={userRoles} />
        </div>
    );
}
