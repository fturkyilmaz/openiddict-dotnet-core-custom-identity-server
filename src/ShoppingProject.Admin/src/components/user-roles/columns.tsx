"use client";

import type { ColumnDef } from "@tanstack/react-table";
import { EllipsisVertical } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";

import type { UserRole } from "./types";

export const userRoleColumns: ColumnDef<UserRole>[] = [
    {
        accessorKey: "id",
        header: "ID",
        cell: ({ row }) => <span className="tabular-nums">{row.original.id}</span>,
    },
    {
        accessorKey: "user",
        header: "User",
        cell: ({ row }) => {
            const user = row.original.user;
            return <span className="font-medium">{user}</span>;
        },
    },
    {
        accessorKey: "role",
        header: "Role",
        cell: ({ row }) => {
            const role = row.original.role;
            return (
                <Badge variant="default" className="capitalize">
                    {role}
                </Badge>
            );
        },
    },
    {
        accessorKey: "userId",
        header: "User ID",
        cell: ({ row }) => <span className="tabular-nums text-muted-foreground text-xs">{row.original.userId}</span>,
    },
    {
        accessorKey: "roleId",
        header: "Role ID",
        cell: ({ row }) => <span className="tabular-nums text-muted-foreground text-xs">{row.original.roleId}</span>,
    },
    {
        id: "actions",
        cell: ({ row }) => (
            <DropdownMenu>
                <DropdownMenuTrigger asChild>
                    <Button variant="ghost" className="flex size-8 text-muted-foreground" size="icon">
                        <EllipsisVertical />
                        <span className="sr-only">Open menu</span>
                    </Button>
                </DropdownMenuTrigger>
                <DropdownMenuContent align="end" className="w-32">
                    <DropdownMenuItem onSelect={() => console.log("Edit", row.original)}>Edit</DropdownMenuItem>
                    <DropdownMenuItem onSelect={() => console.log("View Details", row.original)}>View Details</DropdownMenuItem>
                    <DropdownMenuSeparator />
                    <DropdownMenuItem variant="destructive" onSelect={() => console.log("Delete", row.original)}>Delete</DropdownMenuItem>
                </DropdownMenuContent>
            </DropdownMenu>
        ),
    },
];
