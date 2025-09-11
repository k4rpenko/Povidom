import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CheckCoockieService {
  http = inject(HttpClient)
  constructor() { }

  PutCheckCoockie() {
    return this.http.put<{ cookie: string }>(`api/AccountSettings/SessionsUpdate`, {}, {
      headers: { 'Content-Type': 'application/json' },
      withCredentials: true
    });
  }
}
