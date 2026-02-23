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


  loadPosts(): Promise<void> {
    return new Promise((resolve) => {
      if (this.isLoaded) {
        this.Rest.GetPost().subscribe(res => {
          this.postsSubject.next(res.post);
          this.isLoaded = false;
          resolve(); 
        });
      } else {
        resolve();
      }
    });
  }

  loadFollowingPosts(): Promise<void>{
    return new Promise((resolve) => {
      if (this.isFollowingLoaded) {
        this.Rest.GetFollowingPosts().subscribe(res => {
          this.followingPostsSubject.next(res.post);
          this.isFollowingLoaded = false;
          resolve();
        });
      } else {
        resolve();
      }
    });
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
    const updateListIfContains = (list: Post[]): Post[] => {
      if (!list.some(p => p.id === post.id)) return list;
      return list.map(p => p.id === post.id ? { ...post } : p);
    };

    this.postsSubject.next(updateListIfContains(this.postsSubject.value));
    this.followingPostsSubject.next(updateListIfContains(this.followingPostsSubject.value));
  }

}