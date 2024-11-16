import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { CheckUser } from '../../Global';

@Injectable({
  providedIn: 'root'
})
export class updateAccetsToken {
  http = inject(HttpClient)
  constructor() { }

  updateAccetsToken(data: String) {
    const json = {
      accessToken: data
    };
    
    return this.http.put<{ token	: string }>(`api/AccountSettings/TokenUpdate`, json, {
      headers: { 'Content-Type': 'application/json' }
    });
  }
}