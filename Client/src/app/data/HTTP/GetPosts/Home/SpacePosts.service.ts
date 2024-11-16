import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Post, PostArray } from '../../../interface/Post/Post.interface';
import { Observable } from 'rxjs';
import { CheckUser } from '../../../Global';

@Injectable({
  providedIn: 'root'
})
export class SpacePosts {
  http = inject(HttpClient);

  constructor() { }

  getPosts(): Observable<PostArray> {
    return this.http.get<PostArray>(`api/SpacePosts/Home`);
  }
}