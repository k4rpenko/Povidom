import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { CheckCoockieService } from '../api/PUT/verification/checkCoockie/CheckCoockie.service';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(
    private checkCookieService: CheckCoockieService,
    private router: Router,
    private cookieService: CookieService
  ) {}

  canActivate(): Observable<boolean> {
    return this.checkCookieService.PutCheckCoockie().pipe(
      map((response) => {
        // Якщо запит успішний, зберігаємо токен
        const token = response.cookie;
        this.cookieService.set('_ASA', token, undefined, '/', 'localhost', true, 'Strict');
        return true;
      }),
      catchError((error) => {
        // Якщо помилка, перенаправляємо на register
        this.router.navigate(['/register']);
        return of(false);
      })
    );
  }
}
