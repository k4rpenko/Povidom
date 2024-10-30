import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class updateAccetsToken {
  http = inject(HttpClient)
  constructor() { }

  updateAccetsToken(data: String) {
    const json = {
      jwt: data
    };
    
    return this.http.put<{ token	: string }>(`${window.location.origin}/api/Verification/TokenUpdate`, json, {
      headers: { 'Content-Type': 'application/json' }
    });
  }
}