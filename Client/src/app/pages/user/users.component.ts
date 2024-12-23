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
  username: string | null = '';
  UserCache?: User;
  UserData?: UserProfil;
  posts: Post[] = [];
  UrlName: string = '';

  constructor(private route: ActivatedRoute, private router: Router, private cache: MemoryCacheService) {}

  async ngOnInit() {
    this.UrlName = this.route.snapshot.params['username'];
    await this.loadUserCache();
    if(this.UrlName !== this.UserCache?.userName){
      this.loadUserData();
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
      this.cache.setItem("User", this.UserCache);
    });
  }

  private AddCacheUser() {
    return this.GetUserCache.Get().subscribe(response => {
      this.UserCache = response.user;
      this.cache.setItem("User", this.UserCache);
    });
  }


  public Subscribers(){
    return this.SubscriberPut.Put(this.UserData!.id!).subscribe(response => {});
  }
}
