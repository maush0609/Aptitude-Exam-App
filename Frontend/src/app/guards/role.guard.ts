import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean | UrlTree {
    const currentUser = this.authService.currentUserValue;
    const requiredRoles = route.data['roles'] as string[];

    //for debugging
    console.log('[RoleGuard] Checking access for:', state.url);
    console.log('[RoleGuard] Required roles:', requiredRoles);
    console.log('[RoleGuard] Current user:', currentUser);

    if (!currentUser) {
      console.warn('[RoleGuard] No user - redirecting to login');
      return this.router.createUrlTree(['/login'], {
        queryParams: { returnUrl: state.url }
      });
    }

    if (!requiredRoles || requiredRoles.includes(currentUser.role)) {
      return true;
    }

    console.warn(`[RoleGuard] Access denied. Required: ${requiredRoles} | Has: ${currentUser.role}`);
    return this.router.createUrlTree(['/user/dashboard']);
  }
}