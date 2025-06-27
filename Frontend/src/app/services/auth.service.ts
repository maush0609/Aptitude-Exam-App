import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { jwtDecode } from 'jwt-decode';

interface User {
  token: string;
  email: string;
  role: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5069/api/auth';
  private currentUserSubject: BehaviorSubject<User | null>;
  public currentUser: Observable<User | null>;

  constructor(private http: HttpClient, private router: Router) {
    this.currentUserSubject = new BehaviorSubject<User | null>(this.getValidUserFromStorage());
    this.currentUser = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): User | null {
    return this.currentUserSubject.value;
  }

  public get currentRole(): string | null {
    return this.currentUserValue?.role || null;
  }

  public get token(): string | null {
    return this.currentUserValue?.token || null;
  }

  private getValidUserFromStorage(): User | null {
    const userJson = localStorage.getItem('currentUser');
    if (!userJson) return null;
    
    try {
      const user = JSON.parse(userJson);
      return !this.isTokenExpired(user.token) ? user : null;
    } catch {
      return null;
    }
  }

  private isTokenExpired(token: string): boolean {
    try {
      const decoded: any = jwtDecode(token);
      return decoded.exp < Date.now() / 1000;
    } catch {
      return true;
    }
  }

  login(email: string, password: string): Observable<User> {
    return this.http.post<User>(`${this.apiUrl}/login`, { email, password }).pipe(
      tap(response => this.handleAuthentication(response))
    );
  }

  register(firstName: string, lastName: string, email: string, password: string): Observable<User> {
    return this.http.post<User>(`${this.apiUrl}/register`, { firstName, lastName, email, password }).pipe(
      tap(response => this.handleAuthentication(response))
    );
  }

  private handleAuthentication(response: any): void {
    if (!response?.token) return;

    const decoded: any = jwtDecode(response.token);
    const user: User = {
      token: response.token,
      email: decoded.email || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
      role: decoded.role || decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || response.role
    };

    localStorage.setItem('currentUser', JSON.stringify(user));
    this.currentUserSubject.next(user);

    const targetRoute = user.role === 'Admin' ? '/admin/dashboard' : '/user/dashboard';
    this.router.navigate([targetRoute]);
  }

  logout(): void {
    localStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }
}