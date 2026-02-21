import { z } from "zod";

export const userSchema = z.object({
  id: z.string(),
  name: z.string().min(2, "Name must be at least 2 characters"),
  email: z.string().email("Invalid email address"),
  avatar: z.string().optional(),
  role: z.enum(["admin", "user", "editor"]),
  status: z.enum(["active", "inactive"]),
  createdAt: z.date(),
});

export const addUserSchema = z.object({
  name: z.string().min(2, "Name must be at least 2 characters"),
  email: z.string().email("Invalid email address"),
  role: z.enum(["admin", "user", "editor"], {
    required_error: "Please select a role",
  }),
});

export type AddUserInput = z.infer<typeof addUserSchema>;
export type User = z.infer<typeof userSchema>;
