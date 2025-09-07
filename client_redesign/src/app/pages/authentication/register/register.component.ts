import {Component, inject, OnInit} from '@angular/core';
import {Title} from '@angular/platform-browser';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {NgIf} from '@angular/common';
import {Router} from '@angular/router';
import {CookieService} from 'ngx-cookie-service';
import {RegisterService} from '../../../api/POST/Authentication/Register/Register.service';


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

  // Очистити помилку коли паролі співпадають
  onPasswordChange(): void {
    if (this.password === this.password2 && this.password !== '' && this.password2 !== '') {
      this.Error = ''; // Очищаємо помилку якщо паролі співпадають
    }
  }

  onSubmit(): void {
    // Очищаємо помилки при натисканні кнопки
    this.Error = '';
    
    // Валідація email
    if (!this.email || this.email.trim() === '') {
      this.Error = 'Email is required';
      return;
    }
    
    // Валідація паролів
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
      return; // Не перенаправляємо, якщо паролі не співпадають
    }
    
    this.Rest.PostRegister(this.email, this.password).subscribe({
      next: async (response) => {
        const token = response.cookie;
        this.cookieService.set('_ASA', token, undefined, '/', 'localhost', true, 'Strict');
        this.router.navigate(['home']);
      },
      error: (error) => {
        if (error.status === 429) {
          this.Error = 'Too many requests. Please try again later.';
        } else if (error.status === 500) {
          this.Error = 'Internal Server Error';
        } else {
          const errorMessage = error.error?.message || error.message;
          this.Error = errorMessage;
        }
        // Не перенаправляємо при помилці
      }
    });
  }
}
