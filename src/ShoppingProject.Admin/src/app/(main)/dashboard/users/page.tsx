"use client";

import * as React from "react";
import { Plus } from "lucide-react";

import { Button } from "@/components/ui/button";

import { AddUserSheet } from "@/components/users/add-user-sheet";
import { UserTable } from "@/components/users/user-table";
import type { User } from "@/components/users/types";

// Mock initial data
const initialUsers: User[] = [
  {
    id: "1",
    name: "Furkan Turkyilmaz",
    email: "trkyilmazfurkan@gmail.com",
    avatar: "https://avatars.githubusercontent.com/u/43849669",
    role: "admin",
    status: "active",
    createdAt: new Date("2024-01-15"),
  },
  {
    id: "2",
    name: "John Doe",
    email: "john.doe@example.com",
    role: "user",
    status: "active",
    createdAt: new Date("2024-02-20"),
  },
  {
    id: "3",
    name: "Jane Smith",
    email: "jane.smith@example.com",
    role: "editor",
    status: "inactive",
    createdAt: new Date("2024-03-10"),
  },
  {
    id: "4",
    name: "Mike Johnson",
    email: "mike.johnson@example.com",
    role: "user",
    status: "active",
    createdAt: new Date("2024-04-05"),
  },
  {
    id: "5",
    name: "Sarah Williams",
    email: "sarah.williams@example.com",
    role: "editor",
    status: "active",
    createdAt: new Date("2024-05-12"),
  },
];

export default function UsersPage() {
  const [users, setUsers] = React.useState<User[]>(initialUsers);

  function handleAddUser(user: User) {
    setUsers((prev) => [user, ...prev]);
  }

  return (
    <div className="@container/main flex flex-col gap-4 md:gap-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight">Users</h1>
          <p className="text-muted-foreground text-sm">Manage your team members and their roles.</p>
        </div>
        <AddUserSheet onAddUser={handleAddUser}>
          <Button>
            <Plus className="mr-2 size-4" />
            Add User
          </Button>
        </AddUserSheet>
      </div>
      <UserTable data={users} />
    </div>
  );
}
