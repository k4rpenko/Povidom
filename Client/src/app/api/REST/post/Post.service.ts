import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Post } from '../../../data/interface/Post/Post.interface';

@Injectable({
  providedIn: 'root'
})
export class PostService {
  http = inject(HttpClient)
  constructor() { }

  GetPost() {
    return this.http.get<{post: Post[]}>(`api/SpacePosts/GetPosts`, {
      headers: { 'Content-Type': 'application/json' },
      withCredentials: true
    });
  }

  LikePost(post_id: string) {
    return this.http.put<{ cookie: string }>(
      `api/SpacePosts/LikePost?post_id=${post_id}`,
      null,
      { withCredentials: true }
    );
  }

  AddPost(post: Post) {
    const _data = {
      "_data": {
        post
      }
    };
    
    return this.http.post<{ post: Post }>(`api/SpacePosts/AddPost`, post, {
      headers: { 'Content-Type': 'application/json' },
      withCredentials: true
    });
  }

  DeleteLikePost(post_id: string) {
    return this.http.delete<{ cookie: string }>(
      `api/SpacePosts/LikePost?post_id=${post_id}`,
    );
  }
}
