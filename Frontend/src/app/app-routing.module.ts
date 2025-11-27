import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/login.component';
import { ProjectsComponent } from './components/projects.component';
import { ProjectDetailComponent } from './components/project-detail.component';
import { AuthGuard } from './guards/auth.guard';

const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: LoginComponent },
  { 
    path: 'projects', 
    component: ProjectsComponent,
    canActivate: [AuthGuard]
  },
  { 
    path: 'projects/:id', 
    component: ProjectDetailComponent,
    canActivate: [AuthGuard]
  },
  { path: '**', redirectTo: '/login' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }