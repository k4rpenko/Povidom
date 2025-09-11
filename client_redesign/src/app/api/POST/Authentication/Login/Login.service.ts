import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LoginService {
  http = inject(HttpClient)
  constructor() { }

  PostLogin(email: String, password: String) {
    const json = {
      "email": email,
      "password": password
    };

    return this.http.post<{ cookie	: string }>(`api/Auth/login`, json, {
      headers: { 'Content-Type': 'application/json' },
      withCredentials: true
    });
  }
}
