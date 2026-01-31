import { Component, inject } from '@angular/core';
import { HEADERComponent } from "../../components/header/header.component";
import { BorderMainComponent } from "../../components/border-main/border-main.component";
import { Post } from '../../data/interface/Post/Post.interface';
import { Router, RouterModule } from '@angular/router';
import { PostService } from '../../api/REST/post/Post.service';
import { PostCacheService } from '../../data/cache/post.service';
import { CommonModule } from '@angular/common';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-saved',
  imports: [HEADERComponent, CommonModule, BorderMainComponent, RouterModule],
  templateUrl: './saved.component.html',
  styleUrl: './saved.component.scss'
})
export class SavedComponent {
  Rest = inject(PostService);
  
  constructor(private router: Router, private postsService: PostService, public postCache: PostCacheService) {}
  loading: boolean = true;
  posts: Post[] = [];
  
  ngOnInit() {
    if (this.posts.length === 0) {
      this.getPosts();
    }
  }

  getPosts() {
    this.Rest.GetSavedPost().pipe(finalize(() => {this.loading = false;})).subscribe({
      next: (res) => {
        this.posts = res.post;
      },
      error: (error) => {
        console.error(error);
      }
    });
  }

  navigateToPost(id: string) {
    this.router.navigate(['/post', id])
  }

  navigateToPostreply = () =>{
    console.log(2)
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
