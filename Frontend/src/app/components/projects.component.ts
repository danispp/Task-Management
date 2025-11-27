import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ProjectService } from '../services/project.service';
import { AuthService } from '../services/auth.service';
import { Project } from '../models/models';

@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.css']
})
export class ProjectsComponent implements OnInit {
  projects: Project[] = [];
  loading = true;
  showCreateForm = false;
  newProjectName = '';
  newProjectDescription = '';

  constructor(
    private projectService: ProjectService,
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadProjects();
  }

  loadProjects(): void {
    this.loading = true;
    this.projectService.getProjects().subscribe({
      next: (projects: Project[]) => {
        this.projects = projects;
        this.loading = false;
      },
      error: (err: any) => {
        console.error('Error loading projects:', err);
        this.loading = false;
      }
    });
  }

  createProject(): void {
    if (!this.newProjectName.trim()) {
      return;
    }

    this.projectService.createProject({
      name: this.newProjectName,
      description: this.newProjectDescription || undefined
    }).subscribe({
      next: () => {
        this.newProjectName = '';
        this.newProjectDescription = '';
        this.showCreateForm = false;
        this.loadProjects();
      },
      error: (err: any) => {
        console.error('Error creating project:', err);
        alert('Failed to create project');
      }
    });
  }

  viewProject(id: number): void {
    this.router.navigate(['/projects', id]);
  }

  deleteProject(id: number, event: Event): void {
    event.stopPropagation();

    if (confirm('Are you sure you want to delete this project?')) {
      this.projectService.deleteProject(id).subscribe({
        next: () => {
          this.loadProjects();
        },
        error: (err: any) => {
          console.error('Error deleting project:', err);
          alert('Failed to delete project');
        }
      });
    }
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}