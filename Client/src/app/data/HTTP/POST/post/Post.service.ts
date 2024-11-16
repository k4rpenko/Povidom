import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { CheckUser } from '../../../Global';
import { Post } from '../../../interface/Post/Post.interface';

@Injectable({
  providedIn: 'root'
})
export class SendPost {
  http = inject(HttpClient)
  constructor() { }

  SendPost(post: Post) {
    return this.http.post(`api/SpacePosts/AddPost`, post, {
      headers: { 'Content-Type': 'application/json' }
  });
  }
}
