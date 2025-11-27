import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './components/login.component';
import { ProjectsComponent } from './components/projects.component';

import { JwtInterceptor } from './services/jwt.interceptor';


@NgModule({
  // Declare all components
  declarations: [
    AppComponent,
    LoginComponent,
    ProjectsComponent
  ],
  // Import Angular and third-party modules
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,              // For two-way binding
    ReactiveFormsModule,      // For reactive forms 
    HttpClientModule,         // For HTTP requests
    AppRoutingModule          // For routing
  ],
  // Register HTTP interceptors
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
