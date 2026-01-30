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

  GetPostById(post_id: string) {
    return this.http.get<{post: Post}>(`api/SpacePosts/GetPostsById?post_id=${post_id}`, {
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

  DeleteLikePost(post_id: string) {
    return this.http.delete<{ cookie: string }>(
      `api/SpacePosts/LikePost?post_id=${post_id}`,
    );
  }

  AddPost(post: Post) {    
    return this.http.post<{ post: Post }>(`api/SpacePosts/AddPost`, post, {
      headers: { 'Content-Type': 'application/json' },
      withCredentials: true
    });
  }

  AddComents(post: Post) {
    return this.http.put<{ cookie: string }>(`api/SpacePosts/Comment`, post,
      { withCredentials: true }
    );
  }
}
