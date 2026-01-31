import {Component, inject, OnInit} from '@angular/core';
import {Title} from '@angular/platform-browser';
import {FormsModule} from '@angular/forms';
import {NgIf} from '@angular/common';
import { LoginService } from '../../../api/POST/Authentication/Login/Login.service';
import {Router} from '@angular/router';
import {CookieService} from 'ngx-cookie-service';


@Component({
  selector: 'app-login',
  imports: [
    FormsModule,
    NgIf
  ],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  Rest = inject(LoginService);
  constructor(private titleService: Title, private router: Router, private cookieService: CookieService) { }

  ngOnInit() {
    this.titleService.setTitle('Authentication');
  }

  email: string = '';
  password: string = '';
  Error: string = '';

  onSubmit(): void {
    this.Error = '';
    

    if (!this.email || this.email.trim() === '') {
      this.Error = 'Email is required';
      return;
    }
    

    if (!this.password || this.password.trim() === '') {
      this.Error = 'Password is required';
      return;
    }

    if (!this.isValidEmail(this.email)) {
      this.Error = 'Please enter a valid email address.';
      return;
    }

    if (!this.isValidPassword(this.password)) {
      this.Error = 'The password must contain at least 6 characters, including letters and numbers.';
      return;
    }
    
    this.Rest.PostLogin(this.email, this.password).subscribe({
      next: (response) => {
        const token = response.cookie;
        this.cookieService.set('_ASA', token, undefined, '/', 'localhost', true, 'Strict');
        this.router.navigate(['home']);
      },
      error: (error) => {
        if (error.status === 429) {
          this.Error = 'Too many requests. Please try again later.';
        } else {
          const errorMessage = error.error?.message || error.message;
          this.Error = errorMessage;
        }
      }
    });
  }

  isValidEmail(email: string): boolean {
    const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    return emailPattern.test(email);
  }

  isValidPassword(password: string): boolean {
    const hasUpperCase = /[A-Z]/.test(password); 
    const hasLowerCase = /[a-z]/.test(password); 
    const hasNumber = /\d/.test(password); 
    const isLongEnough = password.length >= 6; 
  
    return hasUpperCase && hasLowerCase && hasNumber && isLongEnough; 
  }

  onGoogleLogin(): void {
    window.location.href = 'api/GoogleAuthentication/GoogleAuth';
  }
}
