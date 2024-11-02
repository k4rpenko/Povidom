import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Post, PostArray } from '../../../interface/Post/Post.interface';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SpacePosts {
  http = inject(HttpClient);

  constructor() { }

  getPosts(): Observable<PostArray> {
    return this.http.get<PostArray>(/*`${window.location.origin}/api/AccountSettings/ConfirmationAccount`*/ "https://localhost:55225/api/SpacePosts/Home");
  }
}



