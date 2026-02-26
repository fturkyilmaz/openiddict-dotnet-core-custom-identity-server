"use client";

import * as React from "react";
import { Search } from "lucide-react";

import { Input } from "@/components/ui/input";

import { DataTable } from "../data-table/data-table";
import { DataTablePagination } from "../data-table/data-table-pagination";
import { useDataTableInstance } from "@/hooks/use-data-table-instance";

import { userRoleColumns } from "./columns";
import type { UserRole } from "./types";

interface UserRoleTableProps {
    data: UserRole[];
}

export function UserRoleTable({ data }: UserRoleTableProps) {
    const [globalFilter, setGlobalFilter] = React.useState("");

    const filteredData = React.useMemo(() => {
        if (!globalFilter) return data;
        const searchLower = globalFilter.toLowerCase();
        return data.filter(
            (userRole) =>
                userRole.user.toLowerCase().includes(searchLower) ||
                userRole.role.toLowerCase().includes(searchLower) ||
                userRole.id.toLowerCase().includes(searchLower),
        );
    }, [data, globalFilter]);

    const filteredTable = useDataTableInstance({
        data: filteredData,
        columns: userRoleColumns,
        getRowId: (row) => row.id,
    });

    return (
        <div className="flex flex-col gap-4">
            <div className="flex items-center gap-4">
                <div className="relative flex-1 max-w-sm">
                    <Search className="absolute left-3 top-1/2 -translate-y-1/2 size-4 text-muted-foreground" />
                    <Input
                        placeholder="Search user roles..."
                        value={globalFilter}
                        onChange={(e) => setGlobalFilter(e.target.value)}
                        className="pl-9"
                    />
                </div>
            </div>
            <div className="overflow-hidden rounded-lg border">
                <DataTable table={filteredTable} columns={userRoleColumns} />
            </div>
            <DataTablePagination table={filteredTable} />
        </div>
    );
}
