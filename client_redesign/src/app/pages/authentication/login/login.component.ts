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
    // Очищаємо помилки при натисканні кнопки
    this.Error = '';
    
    // Валідація email
    if (!this.email || this.email.trim() === '') {
      this.Error = 'Email is required';
      return;
    }
    
    // Валідація пароля
    if (!this.password || this.password.trim() === '') {
      this.Error = 'Password is required';
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

  onGoogleLogin(): void {
    // Прямий редирект на сервер, який поверне Google OAuth URL
    window.location.href = 'api/GoogleAuthentication/GoogleAuth';
  }
}
