import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router } from '@angular/router';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { CheckCoockieService } from '../api/PUT/verification/checkCoockie/CheckCoockie.service';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  token = new BehaviorSubject<boolean>(false);

  constructor(
    private checkCookieService: CheckCoockieService,
    private router: Router,
    private cookieService: CookieService
  ) {}

  canActivate(route: ActivatedRouteSnapshot): Observable<boolean> {
    const isUsernameRoute = route.paramMap.has('username');
    if (this.cookieService.check('_ASA')) {
      this.token.next(true);
    }
    
    return this.checkCookieService.PutCheckCoockie().pipe(
      map((response) => {
        return true;
      }),
      catchError((error) => {
        if(isUsernameRoute) return of(true);
        
        this.router.navigate(['/register']);
        return of(false);
      })
    );
  }
}
