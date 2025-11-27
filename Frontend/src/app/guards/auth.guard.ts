import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';


@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(
    private router: Router,
    private authService: AuthService
  ) { }


   //Returns true if access is allowed, false if denied
   
  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    // Check if user is logged in
    if (this.authService.isLoggedIn) {
      return true; // Allow access
    }

    // User not logged in, redirect to login page
    // Save the URL they were trying to access
    this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
    return false; // Deny access
  }
}
