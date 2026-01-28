import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { UserProfil } from '../interface/Users/UserProfil.interface';
import { UserREST } from '../../api/GET/User/UserData.service';
import { shareReplay, switchMap } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class UserCacheService {

  private user$!: Observable<UserProfil>;
  public isLoaded = false;

  constructor(private Rest: UserREST) {}


  loadUser(): Observable<UserProfil> {
    if (!this.isLoaded) {
      this.user$ = this.Rest.GetUserData().pipe(
        switchMap(res => of(res.user)),
        shareReplay(1)
      );
      this.isLoaded = true;
    }

    return this.user$;
  }


  getUser(): Observable<UserProfil> {
    return this.user$;
  }
}
