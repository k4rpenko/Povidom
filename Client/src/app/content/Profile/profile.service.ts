import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ProfileService {
  private baseUrl = 'api/Admin'; //Треба замінити на URL нашої бази даних  

  constructor(private http: HttpClient) {}

  updateUserProfile(userData: any) {
    return this.http.put(`${this.baseUrl}/ChangUser`, userData);
  }
}
