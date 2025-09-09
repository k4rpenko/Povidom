import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class GoogleService {
  http = inject(HttpClient)
  constructor() { }

  SignGoogleOAuth() {

    return this.http.get(`api/GoogleAuthentication/GoogleAuth`, {
      headers: { 'Content-Type': 'application/json' }
    });
  }
}
