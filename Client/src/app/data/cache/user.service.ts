import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { UserProfil } from '../interface/Users/UserProfil.interface';
import { UserREST } from '../../api/REST/user/UserData.service';
import { shareReplay, switchMap } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class UserCacheService {

  private userSubject = new BehaviorSubject<UserProfil | null>(null);
  user$ = this.userSubject.asObservable();
  public isLoaded = false;

  constructor(private Rest: UserREST) {}


  loadUser(): Observable<UserProfil> {
    if (!this.userSubject.value) {
      return this.Rest.GetUserData().pipe(
        switchMap(res => {
          this.userSubject.next(res.user);
          return of(res.user);
        })
      );
    }
    return of(this.userSubject.value);
  }


  getUser(): Observable<UserProfil> {
    return this.user$ as Observable<UserProfil>;
  }

  isUserLoaded(): boolean {
    return !!this.userSubject.value;
  }

  checkUserName(usernameFromUrl: string): boolean {
    const user = this.userSubject.value;
    return !!user && user.userName === usernameFromUrl;
  }
}

function delay(ms: number) {
    return new Promise( resolve => setTimeout(resolve, ms) );
}
