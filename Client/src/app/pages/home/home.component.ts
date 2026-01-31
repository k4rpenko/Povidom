import { Component, inject, OnInit } from '@angular/core';
import { HEADERComponent } from '../../components/header/header.component';
import { BorderMainComponent } from '../../components/border-main/border-main.component';
import { Post } from '../../data/interface/Post/Post.interface';
import { PostService } from '../../api/REST/post/Post.service';
import { PostCacheService } from '../../data/cache/post.service';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-home',
  imports: [
    HEADERComponent,
    CommonModule,
    BorderMainComponent,
    RouterModule
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  Rest = inject(PostService);
  
  constructor(private router: Router, private postsService: PostService, public postCache: PostCacheService) {}

  posts: Post[] = [];

  ngOnInit() {
    if (this.posts.length === 0) {
      this.getPosts();
    }
  }

  getPosts() {
    this.postCache.loadPosts();
    this.postCache.postsSubject.subscribe(posts => {
      if(posts.length === 0) return;

      this.posts = posts;
    });
    
  }

  sendPost(post: Post){
    this.postCache.SendPost(post);
    this.postCache.postsSubject.subscribe(posts => { this.posts = posts });
  }


  navigateToPost(id: string) {
    this.router.navigate(['/post', id])
  }

  navigateToPostreply(id: string) {
    console.log(2);
  }

  likePost(id: string) {
    const updatePost = this.posts.find(post => post.id === id)!;
    updatePost.likeAmount = (updatePost?.likeAmount || 0) + 1;
    updatePost.youLike = true;

    this.posts = this.posts.map(post => post.id === id ? updatePost : post);
    this.postCache.changPost(updatePost);
    this.Rest.LikePost(id).subscribe({
      next: () => {},
      error: (error) => {
        const updatePost = this.posts.find(post => post.id === id)!;
        updatePost.likeAmount = (updatePost?.likeAmount || 0) - 1;
        updatePost.youLike = false;

        this.posts = this.posts.map(post => post.id === id ? updatePost : post);
        this.postCache.changPost(updatePost);
      }
    });
  }

  DeleteLikePost(id: string) {
    const updatePost = this.posts.find(post => post.id === id)!;
    updatePost.likeAmount = (updatePost?.likeAmount || 0) - 1;
    updatePost.youLike = false;

    this.posts = this.posts.map(post => post.id === id ? updatePost : post);
    this.postCache.changPost(updatePost);
    this.Rest.DeleteLikePost(id).subscribe({
      next: () => {},
      error: (error) => {
        const updatePost = this.posts.find(post => post.id === id)!;
        updatePost.likeAmount = (updatePost?.likeAmount || 0) + 1;
        updatePost.youLike = true;

        this.posts = this.posts.map(post => post.id === id ? updatePost : post);
        this.postCache.changPost(updatePost);
      }
    });
  }

  savedPost(id: string) {
    const updatePost = this.posts.find(post => post.id === id)!;
    updatePost.youSaved = true;

    this.posts = this.posts.map(post => post.id === id ? updatePost : post);
    this.postCache.changPost(updatePost);
    this.Rest.SavedPost(id).subscribe({
      next: () => {},
      error: (error) => {
        const updatePost = this.posts.find(post => post.id === id)!;
        updatePost.youSaved = false;

        this.posts = this.posts.map(post => post.id === id ? updatePost : post);
        this.postCache.changPost(updatePost);
      }
    });
  }

  UnsavedPost(id: string) {
    const updatePost = this.posts.find(post => post.id === id)!;
    updatePost.youSaved = false;

    this.posts = this.posts.map(post => post.id === id ? updatePost : post);
    this.postCache.changPost(updatePost);
    this.Rest.DeleteSavedPost(id).subscribe({
      next: () => {},
      error: (error) => {
        const updatePost = this.posts.find(post => post.id === id)!;
        updatePost.youSaved = true;

        this.posts = this.posts.map(post => post.id === id ? updatePost : post);
        this.postCache.changPost(updatePost);
      }
    });
  }

  commentPost(id: string){

  }

  toggleRepost(id: string){

  }

  updatePost(i: number, post: Post){
    return post.id
  }
}
