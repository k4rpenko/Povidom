import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { AuthComponent } from '../../content/auth/auth.component';
import { RegisterComponent } from '../../content/auth/register/register.component';
import { CookieService } from 'ngx-cookie-service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit{

  constructor(public dialog: MatDialog, private cookieService: CookieService, private router: Router){
    const authToken = this.cookieService.get('authToken');

    if (authToken) {
      this.router.navigate(['/home']);
    }
  }

  ngOnInit(): void {
    throw new Error('Method not implemented.');
  }

  openAuth(): void {
    this.dialog.open(RegisterComponent, {});
  }

  openLogin(): void {
    this.dialog.open(AuthComponent, {});
  }
}
