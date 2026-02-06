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
import { UsersKnow } from "../../components/user/users-know/users-know";
import { PostComponent } from "../../components/post/post";

@Component({
  selector: 'app-user',
  imports: [
    CommonModule,
    HEADERComponent,
    BorderMainComponent,
    UsersKnow,
    PostComponent
],
  templateUrl: './user.component.html',
  styleUrl: './user.component.scss'
})
export class UserComponent{
  user!: UserProfil;
  arrayType: number = 0;
  Rest = inject(PostService);
  UserRest = inject(UserREST);
  loading: boolean = true;
  posts: Post[] = [];
  You: boolean = false;
  NotFoundUser: boolean = false;
  loadingUser: boolean = true;
  YourUserName: string = "";

  constructor( private userCache: UserCacheService, private activatedRoute: ActivatedRoute, private router: Router, private postsService: PostService, public postCache: PostCacheService, private UserServiceRest: UserREST) {}
  
  
  ngOnInit() {
    this.activatedRoute.params.subscribe(async params => {
      
      const usernameFromUrl = params['username'];
      
      this.loadingUser = true;
      this.posts = [];
      this.NotFoundUser = false;

      const currentUser = await firstValueFrom(this.userCache.loadUser());
      if (currentUser) {
        this.YourUserName = currentUser.userName!;
      }

      if (this.YourUserName === usernameFromUrl) {
        this.You = true;
        this.userCache.getUser().subscribe(user => {
          if (!user) return;
          this.user = user;
          this.loadingUser = false;
          this.getPosts(user.userName!);
        });
      } else {
        this.You = false;
        this.UserRest.GetUserDataNick(usernameFromUrl).subscribe({
          next: (res) => {
            this.user = res.user;
            this.loadingUser = false;
            this.getPosts(this.user.userName!);
          },
          error: (error) => {
            if (error.status === 404) {
              this.NotFoundUser = true;
              this.loadingUser = false;
              this.loading = false;
            }
          }
        });
      }
    });
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


  Subscribers(user_name: string){
    this.user.youFollower = !this.user.youFollower
    this.user.followersAmount!++;
    this.UserRest.PutSubscribers(user_name).subscribe({
      next: () => {
        this.userCache.AddSubscriber(1);
      },
      error: (error) => {
        this.user.youFollower = !this.user.youFollower;
        this.user.followersAmount!--;
      }
    })
  }

  UnSubscribers(user_name: string){
    this.user.youFollower = !this.user.youFollower
    this.user.followersAmount!--;
    this.UserRest.DeleteSubscribers(user_name).subscribe({
      next: () => {
        this.userCache.AddSubscriber(-1);
      },
      error: (error) => {
        this.user.youFollower = !this.user.youFollower;
        this.user.followersAmount!++;
      }
    })
  }

  updatePost(i: number, post: Post){
    return post.id
  }

  isUsersArray = false;

  openUsersArray(type: number) {
    this.arrayType = type;
    this.isUsersArray = true;
  }

  closeUsersArray() {
    this.isUsersArray = false;
  }
}
