import { Component, inject, OnInit } from '@angular/core';
import {HEADERComponent} from '../../components/header/header.component';
import {BorderMainComponent} from '../../components/border-main/border-main.component';
import { UserProfil } from '../../data/interface/Users/UserProfil.interface';
import {CommonModule } from '@angular/common';
import { UserCacheService } from '../../data/cache/user.service';
import { Post } from '../../data/interface/Post/Post.interface';
import { PostService } from '../../api/REST/post/Post.service';
import { Router, ActivatedRoute  } from '@angular/router';
import { PostCacheService } from '../../data/cache/post.service';
import { finalize, firstValueFrom } from 'rxjs';
import { UserREST } from '../../api/REST/user/UserData.service';

@Component({
  selector: 'app-user',
  imports: [
    CommonModule,
    HEADERComponent,
    BorderMainComponent
  ],
  templateUrl: './user.component.html',
  styleUrl: './user.component.scss'
})
export class UserComponent{
  user!: UserProfil;
  Rest = inject(PostService);
  UserRest = inject(UserREST);
  loading: boolean = true;
  posts: Post[] = [];
  You: boolean = false;
  NotFoundUser: boolean = false;
  loadingUser: boolean = true;

  constructor( private userCache: UserCacheService, private activatedRoute: ActivatedRoute, private router: Router, private postsService: PostService, public postCache: PostCacheService, private UserServiceRest: UserREST) {}
  
  

  
  async ngOnInit() {
    const usernameFromUrl = this.activatedRoute.snapshot.paramMap.get('username')!;
    const user = await firstValueFrom(this.userCache.loadUser());
    
    if (user.userName === usernameFromUrl){
      this.userCache.getUser().subscribe(user => {
        if (!user) return;
        this.user = user;
        this.loadingUser = false;
      });
      

      if (this.posts.length === 0) {
        this.getPosts(user.userName);
      }
    }
    else{
      this.UserRest.GetUserDataNick(usernameFromUrl).subscribe({
        next: (res) => {
          this.user = res.user;
          this.loadingUser = false;
          if (this.posts.length === 0) {
            this.getPosts(this.user.userName!);
          }
        },
        error: (error) => {
          if (error.status === 404) {
            this.NotFoundUser = true;
            this.loadingUser = false;
            this.loading = false;
          }

          //console.error(error);
        }
      })
    }
  }

  getPosts(user: string) {
    this.UserRest.GetUserPosts(user).pipe(finalize(() => { this.loading = false; })).subscribe({
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
