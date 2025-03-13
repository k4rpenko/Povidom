import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CheckCoockieService {
  http = inject(HttpClient)
  constructor() { }

  GetCheckCoockie(email: String, password: String) {
    return this.http.get<{ cookie	: string }>(``, {
      headers: { 'Content-Type': 'application/json' }
    });
  }
}
