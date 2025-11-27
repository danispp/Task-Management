

// User model
export interface User {
  id: number;
  email: string;
  fullName: string;
  createdAt: Date;
}

// Project model
export interface Project {
  id: number;
  name: string;
  description?: string;
  createdAt: Date;
  userId: number;
  taskCount: number;
}

// Detailed project with tasks
export interface ProjectDetail {
  id: number;
  name: string;
  description?: string;
  createdAt: Date;
  user: User;
  tasks: Task[];
}

// Task model
export interface Task {
  id: number;
  title: string;
  description?: string;
  status: TaskStatus;
  priority: Priority;
  createdAt: Date;
  dueDate?: Date;
  projectId: number;
  projectName: string;
  assignedToUser?: User;
}

// Enums for task status and priority
export enum TaskStatus {
  Todo = 0,
  InProgress = 1,
  Done = 2
}

export enum Priority {
  Low = 0,
  Medium = 1,
  High = 2
}

// DTOs for API requests
export interface RegisterRequest {
  email: string;
  password: string;
  fullName: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  user: User;
}

export interface CreateProjectRequest {
  name: string;
  description?: string;
}

export interface UpdateProjectRequest {
  name: string;
  description?: string;
}

export interface CreateTaskRequest {
  title: string;
  description?: string;
  projectId: number;
  priority: Priority;
  dueDate?: Date;
  assignedToUserId?: number;
}

export interface UpdateTaskRequest {
  title: string;
  description?: string;
  status: TaskStatus;
  priority: Priority;
  dueDate?: Date;
  assignedToUserId?: number;
}
