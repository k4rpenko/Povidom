import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { User } from '../../data/interface/Users/AllDataUser.interface';
import { MemoryCacheService } from '../../data/Cache/MemoryCacheService';
import { UserPost } from '../../data/HTTP/GetPosts/User/UserPost.service';
import { Post } from '../../data/interface/Post/Post.interface';
import { CommonModule } from '@angular/common';
import { UserDataGet } from '../../data/HTTP/GetPosts/User/UserDataGet.service';
import { UserProfil } from '../../data/interface/Users/UserProfil.interface';
import { UserChangGet } from '../../data/HTTP/GetPosts/User/UserChangGet.service';
import { Subscribers } from '../../data/HTTP/POST/Subscribers.service';
import { LikePost } from '../../data/HTTP/POST/post/LikePost.service';
import { SpacePostModel } from '../../data/interface/Post/SpacePostModel.interface';
import { CookieService } from 'ngx-cookie-service';


@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './users.component.html',
  styleUrl: './users.component.scss'
})
export class UsersComponent implements OnInit {
  spacePostsService = inject(UserPost);
  UserDataGet = inject(UserDataGet);
  GetUserCache = inject(UserChangGet);
  SubscriberPut = inject(Subscribers);
  spacePostsLikeService = inject(LikePost);
  username: string | null = '';
  UserCache!: User;
  UserData!: UserProfil;
  posts: Post[] = [];
  UrlName: string = '';
  activeTab: string = 'Post';

  constructor(private route: ActivatedRoute, private router: Router, private cache: MemoryCacheService, private cookieService: CookieService) {}

  async ngOnInit() {
    this.UrlName = this.route.snapshot.params['username'];
    await this.loadUserCache();
    if(this.UrlName !== this.UserCache?.userName){
      await this.loadUserData();
    }
  }

  private loadPosts(nick: string) {
    this.spacePostsService.getPosts(nick).subscribe(response => {
      this.posts = response.post;
    });
  }

  private async loadUserCache() {
    try {
      const result = await this.cache.getItem('User');
      if (result == null) {
        await this.AddCacheUser();
      } else {
        this.UserCache = result;
      }
    } catch (error) {
      console.error('Error fetching user data:', error);
    }
  }

  private async loadUserData() {
    return this.UserDataGet.Get(this.UrlName).subscribe(response => {
      this.UserData = response;
    });
  }

  private AddCacheUser() {
    return this.GetUserCache.Get().subscribe(response => {
      this.UserCache = response.user;
      this.cache.setItem("User", this.UserCache);
    });
  }


  public Subscribers(){
    return this.SubscriberPut.Put(this.UserData!.id!).subscribe(response => {
      this.UserData!.youFollower = true;
    });
  }


  public DeleteSubscribers(){
    return this.SubscriberPut.Delete(this.UrlName, this.cookieService.get('authToken')).subscribe(response => {
      this.UserData!.youFollower = false;
    });
  }

  setActiveTab(tab: string) {
    this.activeTab = tab;
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
        const likedPost = this.UserData.post!.find(p => p.id === post.id);
        likedPost!.likeAmount = Math.max(0, (likedPost!.likeAmount || 0) - 1);
        likedPost!.youLike = false;
      },
      error: (error) => console.error('Error unliking post:', error)
    });
  }

  LikePost(postId: Post){
    const postlike: SpacePostModel = {
      Id: postId.id!,
      UserId: this.cookieService.get('authToken')
    }

    this.spacePostsLikeService.Like(postlike).subscribe({
      next: (response) => {
        const likedPost = this.UserData.post!.find(p => p.id === postId.id);
        likedPost!.likeAmount! += 1;
        likedPost!.youLike = true;
      },
      error: (error) => console.error('Error liking post:', error)
    });
  }

}
