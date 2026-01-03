import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, tap } from 'rxjs';

export interface User {
    username: string;
    role: 'Admin' | 'User';
    token: string;
}

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private apiUrl = 'http://localhost:5037/api/auth'; // Adjust port matching backend
    private currentUserSubject = new BehaviorSubject<User | null>(this.getUserFromStorage());
    public currentUser$ = this.currentUserSubject.asObservable();

    constructor(private http: HttpClient, private router: Router) { }

    public get currentUserValue(): User | null {
        return this.currentUserSubject.value;
    }

    private getUserFromStorage(): User | null {
        const user = localStorage.getItem('currentUser');
        return user ? JSON.parse(user) : null;
    }

    login(credentials: { username: string; password?: string; googleToken?: string }): Observable<any> {
        // For Admin (username/password)
        if (credentials.password) {
            return this.http.post<any>(`${this.apiUrl}/login`, credentials).pipe(
                tap(response => {
                    // Mocking response structure if backend differs, but assuming returns { token, role, ... }
                    const user: User = {
                        username: credentials.username,
                        role: 'Admin',
                        token: response.token
                    };
                    this.storeUser(user);
                })
            );
        }
        // For User (Google)
        else if (credentials.googleToken) {
            return this.http.post<any>(`${this.apiUrl}/google-login`, { token: credentials.googleToken }).pipe(
                tap(response => {
                    const user: User = {
                        username: response.email, // Assuming backend returns email
                        role: 'User',
                        token: response.token
                    };
                    this.storeUser(user);
                })
            );
        }
        throw new Error('Invalid credentials');
    }

    logout() {
        localStorage.removeItem('currentUser');
        this.currentUserSubject.next(null);
        this.router.navigate(['/login']);
    }

    private storeUser(user: User) {
        localStorage.setItem('currentUser', JSON.stringify(user));
        this.currentUserSubject.next(user);
    }
}
