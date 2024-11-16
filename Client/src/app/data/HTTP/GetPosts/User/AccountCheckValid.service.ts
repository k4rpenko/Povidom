import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { CheckUser } from '../../../Global';

@Injectable({
  providedIn: 'root'
})
export class EmailValid {
  http = inject(HttpClient)
  constructor() { }

  PostValidToken(data: String) {
    const json = {
      jwt: data
    };
    return this.http.post(`api/SpacePosts/${null}`, json, {
      headers: { 'Content-Type': 'application/json' }
  });
  }
}
