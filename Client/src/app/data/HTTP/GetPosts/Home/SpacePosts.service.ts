import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { post } from '../../../interface/Post/Post.interface';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SpacePosts {
  http = inject(HttpClient)
  constructor() { }

  Posts() {
    return this.http.get<post>(/*`${window.location.origin}/api/AccountSettings/ConfirmationAccount`*/ "https://localhost:56730/api/SpacePosts/Home");
  }

  
}
