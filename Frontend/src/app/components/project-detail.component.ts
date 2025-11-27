import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProjectService } from '../services/project.service';
import { TaskService } from '../services/task.service';
import { ProjectDetail, Task, TaskStatus, Priority } from '../models/models';

@Component({
  selector: 'app-project-detail',
  templateUrl: './project-detail.component.html',
  styleUrls: ['./project-detail.component.css']
})
export class ProjectDetailComponent implements OnInit {
  project: ProjectDetail | null = null;
  loading = true;
  showCreateTask = false;
  newTaskTitle = '';
  newTaskDescription = '';
  TaskStatus = TaskStatus;
  Priority = Priority;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private projectService: ProjectService,
    private taskService: TaskService
  ) { }

  ngOnInit(): void {
    const id = this.route.snapshot.params['id'];
    this.loadProject(id);
  }

  loadProject(id: number): void {
    this.loading = true;
    this.projectService.getProject(id).subscribe({
      next: (project: ProjectDetail) => {
        this.project = project;
        this.loading = false;
      },
      error: (err: any) => {
        console.error('Error loading project:', err);
        this.loading = false;
        this.router.navigate(['/projects']);
      }
    });
  }

  createTask(): void {
    if (!this.newTaskTitle.trim() || !this.project) {
      return;
    }

    this.taskService.createTask({
      title: this.newTaskTitle,
      description: this.newTaskDescription || undefined,
      projectId: this.project.id,
      priority: Priority.Medium
    }).subscribe({
      next: () => {
        this.newTaskTitle = '';
        this.newTaskDescription = '';
        this.showCreateTask = false;
        this.loadProject(this.project!.id);
      },
      error: (err: any) => {
        console.error('Error creating task:', err);
        alert('Failed to create task');
      }
    });
  }

  updateTaskStatus(taskId: number, status: TaskStatus): void {
    this.taskService.updateTaskStatus(taskId, status).subscribe({
      next: () => {
        if (this.project) {
          const task = this.project.tasks.find(t => t.id === taskId);
          if (task) {
            task.status = status;
          }
        }
      },
      error: (err: any) => {
        console.error('Error updating task:', err);
        alert('Failed to update task');
      }
    });
  }

  deleteTask(taskId: number): void {
    if (!confirm('Delete this task?')) {
      return;
    }

    this.taskService.deleteTask(taskId).subscribe({
      next: () => {
        if (this.project) {
          this.loadProject(this.project.id);
        }
      },
      error: (err: any) => {
        console.error('Error deleting task:', err);
        alert('Failed to delete task');
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/projects']);
  }

  getStatusClass(status: TaskStatus): string {
    switch (status) {
      case TaskStatus.Todo: return 'status-todo';
      case TaskStatus.InProgress: return 'status-progress';
      case TaskStatus.Done: return 'status-done';
      default: return '';
    }
  }

  getPriorityClass(priority: Priority): string {
    switch (priority) {
      case Priority.Low: return 'priority-low';
      case Priority.Medium: return 'priority-medium';
      case Priority.High: return 'priority-high';
      default: return '';
    }
  }
}