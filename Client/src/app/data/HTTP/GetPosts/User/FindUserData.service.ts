import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { CheckUser } from '../../../Global';
import { Observable } from 'rxjs';
import { userG } from '../../../interface/Users/User.interface';

@Injectable({
  providedIn: 'root'
})
export class FindUserData {
  http = inject(HttpClient);
  
  constructor() { }

  FindUserData(nick: string): Observable<userG> {
    return this.http.get<userG>(`api/Fleets/chat/${nick}`);    
  }
}

