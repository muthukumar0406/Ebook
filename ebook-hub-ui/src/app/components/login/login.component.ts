import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
    selector: 'app-login',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.css']
})
export class LoginComponent {
    username = '';
    password = '';
    errorMessage = '';

    constructor(private authService: AuthService, private router: Router) { }

    loginAdmin() {
        this.authService.login({ username: this.username, password: this.password }).subscribe({
            next: (user) => {
                if (user.role === 'Admin') {
                    this.router.navigate(['/admin']);
                } else {
                    this.errorMessage = 'Not authorized as Admin';
                }
            },
            error: (err) => {
                this.errorMessage = 'Login failed';
                console.error(err);
            }
        });
    }

    // Google Login would typically involve an external library or window object interaction
    // For now, we simulate the button action or assume a wrapper is used.
    loginUserGoogle() {
        // Logic for Google Sign-In
        // Example:
        // google.accounts.id.prompt();
        console.log('Google login clicked');
        alert('Google Sign-In logic to be implemented with actual Client ID');
    }
}
