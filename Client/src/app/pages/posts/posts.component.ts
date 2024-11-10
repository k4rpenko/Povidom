import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Post } from '../../data/interface/Post/Post.interface';
import { SpacePosts } from '../../data/HTTP/GetPosts/Home/SpacePosts.service';
import { GetUserData } from '../../data/HTTP/GetPosts/User/GetUserData.service';
import { SendPost } from '../../data/HTTP/POST/post/Post.service';
import { MemoryCacheService } from '../../content/Cache/MemoryCacheService';
import { User } from '../../data/interface/User.interface';
import { CookieService } from 'ngx-cookie-service';
import { FormsModule } from '@angular/forms';
import { LikePost } from '../../data/HTTP/POST/post/LikePost.service';


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
  GetUserData = inject(GetUserData);
  UserData!: User;
  postContent: string = '';

  constructor(private cache: MemoryCacheService,  private cookieService: CookieService,) { }

  async ngOnInit() {
    this.loadPosts();
    await this.loadUserData();
  }
  
  sendPost() {
    console.log('Post content:', this.postContent);
    
    if (this.postContent.trim()) {
      const postData: Post = { 
        userId: this.cookieService.get('authToken'),
        userNickname: this.UserData.userName,
        userName: this.UserData.firstName,
        userAvatar: this.UserData.avatar ?? 'https://54hmmo3zqtgtsusj.public.blob.vercel-storage.com/avatar/Logo-yEeh50niFEmvdLeI2KrIUGzMc6VuWd-a48mfVnSsnjXMEaIOnYOTWIBFOJiB2.jpg',
        content: this.postContent
      };      
      this.SendPost.SendPost(postData).subscribe(
        response => {
          console.log('Post sent successfully', response);
          this.postContent = '';
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

  LikePost(postId: Post){  
    console.log(postId.id);
    
    const postlike: Post = {
      id: postId.id,
      userId: this.cookieService.get('authToken')
    }


    this.spacePostsLikeService.Like(postlike).subscribe({
      next: (response) => {
        const likedPost = this.posts.find(p => p.id === postId.id);
        if (likedPost && likedPost.like !== undefined) {
          likedPost.like += 1;
        } else if (likedPost) {
          likedPost.like = 1;
        }
      },
      error: (error) => console.error('Error liking post:', error)
    });
  }


  private loadPosts() {
    this.spacePostsService.getPosts().subscribe(response => {
      this.posts = response.post;
    });
  }

  private AddCacheUser() {
    return this.GetUserData.GetUserData().subscribe(response => {
      this.UserData = response.user; 
      this.cache.setItem("User", this.UserData);
    });
  }

  autoResize(event: Event): void {
    const textarea = event.target as HTMLTextAreaElement;
    textarea.style.height = 'auto'; 
    textarea.style.height = `${textarea.scrollHeight}px`; 
  }
}
