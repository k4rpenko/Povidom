import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Post } from '../interface/Post/Post.interface';
import { PostService } from '../../api/REST/post/Post.service';

@Injectable({ providedIn: 'root' })
export class PostCacheService {

  public postsSubject = new BehaviorSubject<Post[]>([]);
  public followingPostsSubject = new BehaviorSubject<Post[]>([]);
  public isLoaded = true;
  public isFollowingLoaded = true;

  constructor(private Rest: PostService) {}


  loadPosts(): void {
    if (this.isLoaded) {
      this.Rest.GetPost().subscribe(res => {
        this.postsSubject.next(res.post);
        this.isLoaded = false;
      });
    }
  }

  loadFollowingPosts(): void{
    if (this.isFollowingLoaded) {
      this.Rest.GetFollowingPosts().subscribe(res => {
        this.followingPostsSubject.next(res.post);
        this.isFollowingLoaded = false;
      });
    }
  }

  addPosts(): void {
    this.Rest.GetPost().subscribe(res => {
      const currentPosts = this.postsSubject.value;
      this.postsSubject.next([...currentPosts, ...res.post]);
    });
  }

  SendPost(post: Post): void {
    const currentPosts = this.postsSubject.value;
    this.postsSubject.next([post, ...currentPosts]);
  }

  changPost(post: Post): void {
    const currentPosts = this.postsSubject.value;
    const updatePost = currentPosts.map( p => p.id === post.id ? post : p);
    this.postsSubject.next(updatePost);
  }

}
