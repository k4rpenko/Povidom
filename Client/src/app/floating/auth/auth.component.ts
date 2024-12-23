import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { jwtDecode } from 'jwt-decode';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { RegisterComponent } from './register/register.component';
import { CheckUser } from '../../data/Global';
import { LoginService } from '../../data/HTTP/POST/Login.service';
import { JwtPayload } from '../../data/interface/Post/JwtPayload.interface';
import { ConfirmationEmail } from '../../data/HTTP/POST/ConfirmationPasswordEmail.service';


@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [  CommonModule, FormsModule],
  providers: [CookieService],
  templateUrl: './auth.component.html',
  styleUrl: './auth.component.scss'
})
export class AuthComponent {
  profileService = inject(LoginService);
  ConfirmationEmail = inject(ConfirmationEmail);

  constructor(public dialog: MatDialog, public dialogRef: MatDialogRef<AuthComponent>, private router: Router, private cookieService: CookieService) {}


  onClose(): void {
    this.dialogRef.close();
  }

  SendEmailConfirmation(){
    this.ConfirmationEmail._ConfirmationEmail(this.emailL).subscribe();
  }

  RegisterForm(){
    this.dialog.open(RegisterComponent, {});
    this.dialogRef.close();
  }

  emailL: string = '';    
  passL: string = '';    
  emailError: string = '';
  passwordError: string = ''; 
  ConfirmationPassword: boolean = false;


  

  onSubmit(): void {
    this.emailError = '';
    this.passwordError = '';

    if (!this.isValidEmail(this.emailL)) {
      this.emailError = 'Будь ласка, введіть коректну електронну пошту.';
    }

    if (!this.isValidPassword(this.passL)) {
      this.passwordError = 'Пароль має містити принаймні 6 символів, включаючи букви та цифри.';
    } 

    if (!this.emailError && !this.passwordError) {

      this.profileService.PostLogin(this.emailL, this.passL).subscribe({
        next: (response) => {
          const token = response.token;
          this.cookieService.set('authToken', token);
          CheckUser.Valid = true;
          const decoded = jwtDecode<JwtPayload>(token);
          this.cookieService.set('Role', decoded.Role);
          window.location.reload()
          this.router.navigate(['/home']);
        },
        error: (error) => {
          if (error.status === 429) {
            this.passwordError = 'Ви перевищили ліміт запитів';
          }
          const message = error.error?.message || 'Сталася помилка, спробуйте ще раз';
          this.passwordError = message;
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