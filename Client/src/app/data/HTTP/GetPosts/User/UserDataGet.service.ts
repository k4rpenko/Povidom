import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Post, PostArray } from '../../../interface/Post/Post.interface';
import { Observable } from 'rxjs';
import { CheckUser } from '../../../Global';
import { UserProfil } from '../../../interface/Users/UserProfil.interface';

@Injectable({
  providedIn: 'root'
})
export class UserDataGet {
  http = inject(HttpClient);

  constructor() { }

  Get(nick: string){
    return this.http.get(`api/Fleets/${nick}`);
  }
}
