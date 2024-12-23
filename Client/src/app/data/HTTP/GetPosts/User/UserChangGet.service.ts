import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { CheckUser } from '../../../Global';
import { Observable } from 'rxjs';
import { user, User } from '../../../interface/Users/AllDataUser.interface';

@Injectable({
  providedIn: 'root'
})
export class UserChangGet {
  http = inject(HttpClient);

  constructor() { }

  Get(): Observable<user> {
    return this.http.get<user>(`api/Fleets`);
  }
}

