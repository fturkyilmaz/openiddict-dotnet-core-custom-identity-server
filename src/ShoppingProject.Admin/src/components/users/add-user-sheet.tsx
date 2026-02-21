"use client";

import * as React from "react";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { Mail, User as UserIcon, UserCog } from "lucide-react";
import { toast } from "sonner";

import { Button } from "@/components/ui/button";
import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetFooter,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from "@/components/ui/sheet";

import { addUserSchema, type AddUserInput } from "./schema";
import type { User } from "./types";

interface AddUserSheetProps {
  onAddUser: (user: User) => void;
  children?: React.ReactNode;
}

export function AddUserSheet({ onAddUser, children }: AddUserSheetProps) {
  const [open, setOpen] = React.useState(false);

  const form = useForm<AddUserInput>({
    resolver: zodResolver(addUserSchema),
    defaultValues: {
      name: "",
      email: "",
      role: undefined,
    },
  });

  function onSubmit(data: AddUserInput) {
    const newUser: User = {
      id: Math.random().toString(36).substring(2, 9),
      ...data,
      status: "active",
      createdAt: new Date(),
    };

    onAddUser(newUser);
    toast.success("User added successfully");
    form.reset();
    setOpen(false);
  }

  return (
    <Sheet open={open} onOpenChange={setOpen}>
      <SheetTrigger asChild>{children}</SheetTrigger>
      <SheetContent side="right" className="flex flex-col sm:max-w-[480px]">
        <SheetHeader className="gap-1.5 px-6 pt-6">
          <div className="flex items-center gap-3">
            <div className="flex size-10 items-center justify-center rounded-full bg-primary/10">
              <UserCog className="size-5 text-primary" />
            </div>
            <div>
              <SheetTitle className="text-xl">Add New User</SheetTitle>
              <SheetDescription className="text-sm">
                Create a new user account for your team.
              </SheetDescription>
            </div>
          </div>
        </SheetHeader>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="flex flex-1 flex-col gap-6 px-6 py-6">
            <div className="flex flex-col gap-5">
              <FormField
                control={form.control}
                name="name"
                render={({ field }) => (
                  <FormItem className="space-y-2.5">
                    <FormLabel className="text-sm font-medium">Full Name</FormLabel>
                    <div className="relative">
                      <UserIcon className="absolute left-3 top-1/2 -translate-y-1/2 size-4 text-muted-foreground" />
                      <FormControl>
                        <Input
                          placeholder="Enter full name"
                          className="pl-10 h-11"
                          {...field}
                        />
                      </FormControl>
                    </div>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="email"
                render={({ field }) => (
                  <FormItem className="space-y-2.5">
                    <FormLabel className="text-sm font-medium">Email Address</FormLabel>
                    <div className="relative">
                      <Mail className="absolute left-3 top-1/2 -translate-y-1/2 size-4 text-muted-foreground" />
                      <FormControl>
                        <Input
                          placeholder="Enter email address"
                          type="email"
                          className="pl-10 h-11"
                          {...field}
                        />
                      </FormControl>
                    </div>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="role"
                render={({ field }) => (
                  <FormItem className="space-y-2.5">
                    <FormLabel className="text-sm font-medium">User Role</FormLabel>
                    <Select onValueChange={field.onChange} defaultValue={field.value}>
                      <FormControl>
                        <SelectTrigger className="h-11">
                          <div className="flex items-center gap-2">
                            <UserCog className="size-4 text-muted-foreground" />
                            <SelectValue placeholder="Select user role" />
                          </div>
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value="admin">
                          <div className="flex items-center gap-2">
                            <span className="font-medium">Admin</span>
                            <span className="text-muted-foreground text-xs">- Full access</span>
                          </div>
                        </SelectItem>
                        <SelectItem value="user">
                          <div className="flex items-center gap-2">
                            <span className="font-medium">User</span>
                            <span className="text-muted-foreground text-xs">- Standard access</span>
                          </div>
                        </SelectItem>
                        <SelectItem value="editor">
                          <div className="flex items-center gap-2">
                            <span className="font-medium">Editor</span>
                            <span className="text-muted-foreground text-xs">- Content access</span>
                          </div>
                        </SelectItem>
                      </SelectContent>
                    </Select>
                    <FormDescription className="text-xs">
                      Select the appropriate role for this user.
                    </FormDescription>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>
            <div className="mt-auto flex items-center gap-3 pt-4">
              <Button
                type="button"
                variant="outline"
                className="flex-1 h-11"
                onClick={() => setOpen(false)}
              >
                Cancel
              </Button>
              <Button type="submit" className="flex-1 h-11">
                Create User
              </Button>
            </div>
          </form>
        </Form>
      </SheetContent>
    </Sheet>
  );
}
