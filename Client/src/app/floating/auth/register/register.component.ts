import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { jwtDecode } from 'jwt-decode';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { RegisterService } from '../../../data/HTTP/POST/Register.service';
import { AuthComponent } from '../auth.component';
import { CheckUser } from '../../../data/Global';
import { JwtPayload } from '../../../data/interface/Post/JwtPayload.interface';


@Component({
  selector: 'app-register',
  standalone: true,
  imports: [  CommonModule, FormsModule],
  providers: [CookieService],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  constructor(public dialog: MatDialog, public dialogRef: MatDialogRef<RegisterComponent>, private router: Router, private cookieService: CookieService) {}

  profileService = inject(RegisterService);
  
  onClose(): void {
    this.dialogRef.close();
  }

  LoginForm(){
    this.dialog.open(AuthComponent, {});
    this.dialogRef.close();
  }

  emailR: string = '';    
  pass1R: string = '';   
  pass2R: string = '';    
  emailError: string = '';
  passwordError: string = ''; 

  onSubmit(): void {
    this.emailError = '';
    this.passwordError = '';

    if (!this.isValidEmail(this.emailR)) {
      this.emailError = 'Будь ласка, введіть коректну електронну пошту.';
    }

    if (!this.isValidPassword(this.pass1R)) {
      this.passwordError = 'Пароль має містити принаймні 6 символів, включаючи букви та цифри.';
    } else if (this.pass1R !== this.pass2R) {
      this.passwordError = 'Паролі не співпадають.';
    }

    if (!this.emailError && !this.passwordError) {
      this.profileService.PostRegister(this.emailR, this.pass1R).subscribe({
        next: async (response) => {
          const token = response.token;
          this.cookieService.set('authToken', token);
          CheckUser.Valid = true;
          const decoded = jwtDecode<JwtPayload>(token);
          this.cookieService.set('UserId', decoded.sub);
          this.cookieService.set('Role', decoded.Role);
          window.location.reload()
          this.router.navigate(['/home']);
        },
        error: (error) => {
          if (error.status === 401 || error.status === 400) {
            this.passwordError = 'Користувач існує';
          }  else if (error.status === 429) {
            this.passwordError = 'Ви перевищили ліміт запитів';
          } else {
            this.passwordError = 'Сталася помилка, спробуйте ще раз';
          }
        }
      });
    }
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
}

function sleep(ms: number) {
  return new Promise(resolve => setTimeout(resolve, ms));
}