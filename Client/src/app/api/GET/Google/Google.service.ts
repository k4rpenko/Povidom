import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class GoogleService {
  http = inject(HttpClient)
  constructor() { }

  SignGoogleOAuth() {
    return this.http.get<{url: string}>(`api/GoogleAuthentication/GoogleAuth`, {
      headers: { 'Content-Type': 'application/json' },
      withCredentials: true
    });
  }
}
