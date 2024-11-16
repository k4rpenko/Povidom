import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { CheckUser } from '../../../Global';
import { Post } from '../../../interface/Post/Post.interface';

@Injectable({
  providedIn: 'root'
})
export class LikePost {
  http = inject(HttpClient)
  constructor() { }

  Like(post: Post) {
    return this.http.put(`api/SpacePosts/LikePost`, post, {
      headers: { 'Content-Type': 'application/json' }
  });
  }
}
