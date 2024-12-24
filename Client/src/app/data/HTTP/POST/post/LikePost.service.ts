import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Post } from '../../../interface/Post/Post.interface';
import { SpacePostModel } from '../../../interface/Post/SpacePostModel.interface';

@Injectable({
  providedIn: 'root'
})
export class LikePost {
  http = inject(HttpClient);

  constructor() { }

  Like(post: Post) {
    return this.http.put(`api/SpacePosts/LikePost`, post, {
      headers: { 'Content-Type': 'application/json' },
      responseType: 'text'
    });
  }

  DeleteLike(post: SpacePostModel) {
    const params = new HttpParams()
      .set('id', post.Id!)
      .set('userId', post.UserId!);

    return this.http.delete(`api/SpacePosts/LikePost`, {
      headers: { 'Content-Type': 'application/json' },
      responseType: 'text',
      params
    });
  }
}
