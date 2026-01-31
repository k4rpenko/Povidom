import {Component, inject, OnInit} from '@angular/core';
import {Title} from '@angular/platform-browser';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import { jwtDecode } from 'jwt-decode';
import {NgIf} from '@angular/common';
import {Router} from '@angular/router';
import {CookieService} from 'ngx-cookie-service';
import {RegisterService} from '../../../api/POST/Authentication/Register/Register.service';
import { JwtPayload } from '../../../data/interface/Post/JwtPayload.interface';


@Component({
  selector: 'app-register',
  imports: [
    ReactiveFormsModule,
    FormsModule,
    NgIf
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent implements OnInit {
  email: string = '';
  password: string = '';
  password2: string = '';
  Error: string = '';
  Rest = inject(RegisterService);

  constructor(private titleService: Title, private router: Router, private cookieService: CookieService) { }

  ngOnInit() {
    this.titleService.setTitle('Authentication');
  }


  onPasswordChange(): void {
    if (this.password === this.password2 && this.password !== '' && this.password2 !== '') {
      this.Error = '';
    }
  }

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
    
    if (!this.password2 || this.password2.trim() === '') {
      this.Error = 'Confirm password is required';
      return;
    }
    
    if(this.password != this.password2) {
      this.Error = 'passwords do not match';
      return;
    }

    if (!this.isValidEmail(this.email)) {
      this.Error = 'Please enter a valid email address.';
      return;
    }

    if (!this.isValidPassword(this.password)) {
      this.Error = 'The password must contain at least 6 characters, including letters and numbers.';
      return;
    } else if (this.password !== this.password2) {
      this.Error = 'Passwords do not match.';
      return;
    }

    this.Rest.PostRegister(this.email, this.password).subscribe({
      next: (response) => {
        const token = response.cookie;
        this.cookieService.set('_ASA', token, undefined, '/', 'localhost', true, 'Strict');
        this.router.navigate(['home']);
      },
      error: (error) => {
        if (error.status === 429) {
          this.Error = 'Too many requests. Please try again later.';
        } 
        else if(error.status === 400){
          this.Error = 'This email address is already taken, please try another one.';
        }
        else {
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
