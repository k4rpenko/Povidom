import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Post } from '../../../data/interface/Post/Post.interface'
import { SpacePosts } from '../../../data/HTTP/GetPosts/Home/SpacePosts.service'

@Component({
  selector: 'app-posts',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './posts.component.html',
  styleUrl: './posts.component.scss'
})
export class PostsComponent {
  posts: Post[] = [];
  spacePostsService = inject(SpacePosts);

  constructor() {
    this.loadPosts();
  }

  loadPosts() {
    this.spacePostsService.getPosts().subscribe(response  => {
      this.posts = response.post;
      console.log(this.posts);
      
    });
  }

  autoResize(event: Event): void {
    const textarea = event.target as HTMLTextAreaElement;
    textarea.style.height = 'auto'; 
    textarea.style.height = `${textarea.scrollHeight}px`; 
  }
}