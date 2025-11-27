import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/login.component';
import { ProjectsComponent } from './components/projects.component';
import { AuthGuard } from './guards/auth.guard';


const routes: Routes = [
  // Default route - projects
  { path: '', redirectTo: '/projects', pathMatch: 'full' },
  
  // Login route - public
  { path: 'login', component: LoginComponent },
  
  // Register route - public
  { path: 'register', component: LoginComponent }, 
  
  { 
    path: 'projects', 
    component: ProjectsComponent,
    canActivate: [AuthGuard]  // User must be logged in
  },
  
  // Wildcard route - redirect to projects
  { path: '**', redirectTo: '/projects' }
];

/**
 * Configure Angular routing
 */
@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
