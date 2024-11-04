import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Post } from '../../data/interface/Post/Post.interface';
import { SpacePosts } from '../../data/HTTP/GetPosts/Home/SpacePosts.service';
import { GetUserData } from '../../data/HTTP/GetPosts/User/GetUserData.service';
import { MemoryCacheService } from '../../content/Cache/MemoryCacheService';
import { User } from '../../data/interface/User.interface';

@Component({
  selector: 'app-posts',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './posts.component.html',
  styleUrls: ['./posts.component.scss']
})
export class PostsComponent implements OnInit {
  posts: Post[] = [];
  spacePostsService = inject(SpacePosts);
  GetUserData = inject(GetUserData);
  UserData!: User;

  constructor(private cache: MemoryCacheService) { }

  async ngOnInit() {
    this.loadPosts();
    await this.loadUserData();
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
