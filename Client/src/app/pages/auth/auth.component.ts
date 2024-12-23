import { Component } from '@angular/core';
import { MatDialogModule } from '@angular/material/dialog';
import { Router, RouterModule } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { MatDialog } from '@angular/material/dialog';
import { RegisterComponent } from '../../floating/auth/register/register.component';
import { AuthComponent } from '../../floating/auth/auth.component';

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [
    MatDialogModule,
    RouterModule
  ],
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.scss'],
})
export class Auth {
  constructor(
    public dialog: MatDialog,
    private cookieService: CookieService,
    private router: Router
  ) {
    const authToken = this.cookieService.get('authToken');
    if (authToken) {
      this.router.navigate(['/home']);
    }
  }

  openAuth(): void {
    this.dialog.open(RegisterComponent, {});
  }

  openLogin(): void {
    this.dialog.open(AuthComponent, {});
  }
}
