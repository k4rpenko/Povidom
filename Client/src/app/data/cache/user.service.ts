import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { UserProfil } from '../interface/Users/UserProfil.interface';
import { UserREST } from '../../api/REST/user/UserData.service';
import { catchError, shareReplay, switchMap } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class UserCacheService {

  private userSubject = new BehaviorSubject<UserProfil | null>(null);
  user$ = this.userSubject.asObservable();
  public isLoaded = false;

  constructor(private Rest: UserREST) {}


  loadUser(): Observable<UserProfil | null> {
    if (!this.userSubject.value) {
      return this.Rest.GetUserData().pipe(
        switchMap(res => {
          this.userSubject.next(res.user);
          return of(res.user);
        }),
        catchError((error) => {
          return of(null);
        })
      );
    }
    return of(this.userSubject.value);
  }

  updateUser(user: UserProfil): void {
    this.userSubject.next(user);
  }

  reloadUser(): Observable<UserProfil> {
    return this.Rest.GetUserData().pipe(
      switchMap(res => {
        this.userSubject.next(res.user);
        return of(res.user);
      })
    );
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

  AddSubscriber(amount: number){
    const user = this.userSubject.value;

    if(!user) return;

    this.userSubject.next({
      ...user,
      subscribersAmount: user.subscribersAmount! + amount
    });
  }
}

function delay(ms: number) {
    return new Promise( resolve => setTimeout(resolve, ms) );
}
