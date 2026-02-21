export type UserRole = "admin" | "user" | "editor";

export type UserStatus = "active" | "inactive";

export interface User {
  id: string;
  name: string;
  email: string;
  avatar?: string;
  role: UserRole;
  status: UserStatus;
  createdAt: Date;
}
