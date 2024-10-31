import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Post, post } from '../../../data/interface/Post/Post.interface'
import { SpacePosts } from '../../../data/HTTP/GetPosts/Home/SpacePosts.service'

@Component({
  selector: 'app-posts',
  standalone: true,
  imports: [ CommonModule ],
  templateUrl: './posts.component.html',
  styleUrl: './posts.component.scss'
})
export class PostsComponent {
  post: Post[] = [];
  profileService = inject(SpacePosts);

  constructor(){ this.loadPosts() }

  loadPosts() {
    this.profileService.Posts().subscribe(response => {
      this.post = response.post;
    });
    console.log(this.post);
    
  }
}
