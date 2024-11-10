import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GetUserData } from '../../data/HTTP/GetPosts/User/GetUserData.service';
import { User } from '../../data/interface/User.interface';
import { MemoryCacheService } from '../Cache/MemoryCacheService';
import { UserPost } from '../../data/HTTP/GetPosts/User/UserPost.service';
import { Post } from '../../data/interface/Post/Post.interface';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [ CommonModule ],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  spacePostsService = inject(UserPost);
  GetUserData = inject(GetUserData);

  username: string | null = '';
  UserData?: User;
  posts: Post[] = [];

  constructor(private route: ActivatedRoute, private cache: MemoryCacheService) {}

  async ngOnInit() {
    await this.loadUserData();
    await this.loadPosts(this.UserData?.userName || "");
  }
  
  private loadPosts(nick: string) {
    this.spacePostsService.getPosts(nick).subscribe(response => {
      this.posts = response.post;
    });
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

  private AddCacheUser() {
    return this.GetUserData.GetUserData().subscribe(response => {
      this.UserData = response.user; 
      this.cache.setItem("User", this.UserData);
    });
  }
}
