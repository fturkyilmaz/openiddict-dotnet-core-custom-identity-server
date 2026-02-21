"use client";

import * as React from "react";
import { Search } from "lucide-react";

import { Input } from "@/components/ui/input";

import { DataTable } from "../data-table/data-table";
import { DataTablePagination } from "../data-table/data-table-pagination";
import { useDataTableInstance } from "@/hooks/use-data-table-instance";

import { userColumns } from "./columns";
import type { User } from "./types";

interface UserTableProps {
  data: User[];
}

export function UserTable({ data: initialData }: UserTableProps) {
  const [data] = React.useState(initialData);
  const [globalFilter, setGlobalFilter] = React.useState("");

  const table = useDataTableInstance({
    data,
    columns: userColumns,
    getRowId: (row) => row.id,
  });

  const filteredData = React.useMemo(() => {
    if (!globalFilter) return data;
    const searchLower = globalFilter.toLowerCase();
    return data.filter(
      (user) =>
        user.name.toLowerCase().includes(searchLower) ||
        user.email.toLowerCase().includes(searchLower) ||
        user.role.toLowerCase().includes(searchLower) ||
        user.status.toLowerCase().includes(searchLower),
    );
  }, [data, globalFilter]);

  const filteredTable = useDataTableInstance({
    data: filteredData,
    columns: userColumns,
    getRowId: (row) => row.id,
  });

  return (
    <div className="flex flex-col gap-4">
      <div className="flex items-center gap-4">
        <div className="relative flex-1 max-w-sm">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 size-4 text-muted-foreground" />
          <Input
            placeholder="Search users..."
            value={globalFilter}
            onChange={(e) => setGlobalFilter(e.target.value)}
            className="pl-9"
          />
        </div>
      </div>
      <div className="overflow-hidden rounded-lg border">
        <DataTable table={filteredTable} columns={userColumns} />
      </div>
      <DataTablePagination table={filteredTable} />
    </div>
  );
}
