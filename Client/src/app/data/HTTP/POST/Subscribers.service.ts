import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { CheckUser } from '../../Global';

@Injectable({
  providedIn: 'root'
})
export class Subscribers {
  http = inject(HttpClient)
  constructor() { }

  Put(id: String) {
    const AccountSettingsModel = {
      id: id
    };

    return this.http.put(`api/Fleets/Subscribers`, AccountSettingsModel, {
      headers: { 'Content-Type': 'application/json' }
    });
  }

  Delete(NickName: string, userId: string) {
    const params = new HttpParams()
      .set('NickName', NickName)
      .set('userId', userId);

    return this.http.delete(`api/Fleets/Subscribers`, {
      headers: { 'Content-Type': 'application/json' },
      responseType: 'text',
      params
    });
  }
}
