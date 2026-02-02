import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Post } from '../../../data/interface/Post/Post.interface';

@Injectable({
  providedIn: 'root'
})
export class PostService {
  GetUserPost(user: string) {
    throw new Error('Method not implemented.');
  }
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
  
  GetSavedPost() {
    return this.http.get<{post: Post[]}>(`api/SpacePosts/GetSavedPosts`, {
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

  LikeComent(post_id: string, coment_id: string) {
    return this.http.put<{ cookie: string }>(
      `api/SpacePosts/LikeComent?post_id=${post_id}&coment_id=${coment_id}`,
      null,
      { withCredentials: true }
    );
  }

  DeleteLikeComent(post_id: string, coment_id: string) {
    return this.http.delete<{ cookie: string }>(
      `api/SpacePosts/LikeComent?post_id=${post_id}&coment_id=${coment_id}`,
    );
  }

  SavedPost(post_id: string) {
    return this.http.put<{ cookie: string }>(
      `api/SpacePosts/SavedPost?post_id=${post_id}`,
      null,
      { withCredentials: true }
    );
  }

  DeleteSavedPost(post_id: string) {
    return this.http.delete<{ cookie: string }>(
      `api/SpacePosts/SavedPost?post_id=${post_id}`,
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