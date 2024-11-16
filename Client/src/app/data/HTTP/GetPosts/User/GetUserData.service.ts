import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { CheckUser } from '../../../Global';
import { Observable } from 'rxjs';
import { user, User } from '../../../interface/User.interface';

@Injectable({
  providedIn: 'root'
})
export class GetUserData {
  http = inject(HttpClient);
  
  constructor() { }

  GetUserData(): Observable<user> {
    return this.http.get<user>(`api/Fleets`);    
  }
}

