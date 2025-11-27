
import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  loading = false;
  error = '';
  isRegisterMode = false;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      fullName: ['']
    });
  }

  ngOnInit(): void {
    this.isRegisterMode = this.router.url.includes('/register');
    
    if (this.isRegisterMode) {
      this.loginForm.get('fullName')?.setValidators([Validators.required]);
    }
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      return;
    }

    this.loading = true;
    this.error = '';

    if (this.isRegisterMode) {
      // REGISTER
      this.authService.register(this.loginForm.value).subscribe({
        next: () => {
          this.router.navigate(['/projects']);
        },
        error: (err: any) => {
          this.error = err.error?.message || 'Registration failed. Please try again.';
          this.loading = false;
        }
      });
    } else {
      // LOGIN
      this.authService.login(this.loginForm.value).subscribe({
        next: () => {
          this.router.navigate(['/projects']);
        },
        error: (err: any) => {
          this.error = err.error?.message || 'Login failed. Please try again.';
          this.loading = false;
        }
      });
    }
  }

  goToRegister(): void {
    this.router.navigate(['/register']);
  }

  goToLogin(): void {
    this.router.navigate(['/login']);
  }
}
