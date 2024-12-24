import { user } from './../../data/interface/Users/AllDataUser.interface';
import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Post } from '../../data/interface/Post/Post.interface';
import { SpacePosts } from '../../data/HTTP/GetPosts/Home/SpacePosts.service';
import { UserChangGet } from '../../data/HTTP/GetPosts/User/UserChangGet.service';
import { SendPost } from '../../data/HTTP/POST/post/Post.service';
import { MemoryCacheService } from '../../data/Cache/MemoryCacheService';
import { User } from '../../data/interface/Users/AllDataUser.interface';
import { CookieService } from 'ngx-cookie-service';
import { FormsModule } from '@angular/forms';
import { LikePost } from '../../data/HTTP/POST/post/LikePost.service';
import { SpacePostModel } from '../../data/interface/Post/SpacePostModel.interface';


@Component({
  selector: 'app-posts',
  standalone: true,
  imports: [ FormsModule, CommonModule],
  templateUrl: './posts.component.html',
  styleUrls: ['./posts.component.scss']
})
export class PostsComponent implements OnInit {
  posts: Post[] = [];
  spacePostsService = inject(SpacePosts);
  spacePostsLikeService = inject(LikePost);
  SendPost = inject(SendPost);
  UserChangGet = inject(UserChangGet);
  UserData!: User;
  postContent: string = '';

  constructor(private cache: MemoryCacheService,  private cookieService: CookieService) { }

  async ngOnInit() {
    this.loadPosts();
    await this.loadUserData();
  }

  sendPost() {

    if (this.postContent.trim()) {
      const postData: SpacePostModel = {
        UserId: this.cookieService.get('authToken'),
        content: this.postContent
      };
      this.SendPost.SendPost(postData).subscribe(
        response => {
          this.postContent = '';
          this.posts.push(response);
        },
        error => {
          console.error('Error sending post', error);
        }
      );
    } else {
      console.log('Post content is empty');
    }
  }

  private async loadUserData() {
    try {
      const result = await this.cache.getItem('User');
      if (result == null) {
        await this.AddCacheUser();
      } else {
        this.UserData = result;
      }
    } catch (error) {
      console.error('Error fetching user data:', error);
    }
  }

  OpenComments(postId: Post){

  }

  LikePost(postId: Post){
    const postlike: SpacePostModel = {
      Id: postId.id!,
      UserId: this.cookieService.get('authToken')
    }

    this.spacePostsLikeService.Like(postlike).subscribe({
      next: (response) => {
        const likedPost = this.posts.find(p => p.id === postId.id);
        likedPost!.likeAmount! += 1;
        likedPost!.youLike = true;
      },
      error: (error) => console.error('Error liking post:', error)
    });
  }

  DeleteLikePost(post: Post) {
    if (!post || !post.id) {
      console.error('Post or Post ID is undefined');
      return;
    }

    const postlike: SpacePostModel = {
      Id: post.id!,
      UserId: this.cookieService.get('authToken')
    };

    this.spacePostsLikeService.DeleteLike(postlike).subscribe({
      next: (response) => {
        const likedPost = this.posts.find(p => p.id === post.id);
        likedPost!.likeAmount = Math.max(0, (likedPost!.likeAmount || 0) - 1);
        likedPost!.youLike = false;
      },
      error: (error) => console.error('Error unliking post:', error)
    });
  }


  private loadPosts() {
    this.spacePostsService.getPosts().subscribe(response => {
      this.posts = response.post;
    });
  }

  private AddCacheUser() {
    return this.UserChangGet.Get().subscribe(response => {
      this.UserData = response.user;
      this.cache.setItem("User", this.UserData);
    });
  }

  autoResize(event: Event): void {
    const textarea = event.target as HTMLTextAreaElement;
    textarea.style.height = 'auto';
    textarea.style.height = `${textarea.scrollHeight}px`;
  }

  handleKeydown(event: KeyboardEvent): void {
    if (event.key === 'Enter' && !event.shiftKey) {
      this.sendPost();
      event.preventDefault();
    }
  }
}
