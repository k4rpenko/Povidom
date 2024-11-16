import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { CheckUser } from '../../Global';

@Injectable({
  providedIn: 'root'
})
export class ConfirmationEmail {
  http = inject(HttpClient)
  constructor() { }

  _ConfirmationEmail(email: String) {
    const json = {
      "email": email,
    };
    return this.http.post(`api/AccountSettings/ConfirmationEmail`, json, {
      headers: { 'Content-Type': 'application/json' }
  });
  }
}
