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
import { empty, finalize, firstValueFrom, take } from 'rxjs';
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
  posts: Post[] = [];

  You = false;
  NotFoundUser = false;
  loadingUser = true;
  loadingPosts = true;
  arrayType = 0;

  YourUserName = '';

  constructor(
    private userCache: UserCacheService,
    private route: ActivatedRoute,
    private userRest: UserREST
  ) {}

  async ngOnInit() {
    const currentUser = await firstValueFrom(this.userCache.loadUser());
    this.YourUserName = currentUser?.userName ?? '';

    this.route.params.subscribe(params => {
      const username = params['username'];

      this.resetState();
      this.You = this.YourUserName === username;

      this.loadPosts(username);

      this.You ? this.bindCurrentUser() : this.loadForeignUser(username);
    });
  }


  private bindCurrentUser() {    
    this.userCache.getUser()
      .pipe(take(1))
      .subscribe(user => {
        if (!user) return;
        this.loadingUser = false;
        this.user = { ...user };
    });
  }

  private loadForeignUser(username: string) {
    this.userRest.GetUserDataNick(username).subscribe({
      next: res => {
        this.user = res.user;
        this.loadingUser = false;
      },
      error: err => {
        if (err.status === 404) {
          this.NotFoundUser = true;
          this.loadingUser = false;
          this.loadingPosts = false;
        }
      }
    });
  }


  private loadPosts(username: string) {
    this.loadingPosts = true;

    this.userRest.GetUserPosts(username)
      .pipe(finalize(() => this.loadingPosts = false))
      .subscribe({
        next: res => this.posts = res.post,
        error: err => console.error(err)
      });
  }

  private optimisticFollow(state: boolean) {
    this.user.youFollower = state;
    this.user.followersAmount! += state ? 1 : -1;
  }

  private resetState() {
    this.user = undefined as any;
    this.posts = [];
    this.NotFoundUser = false;
    this.loadingUser = true;
    this.loadingPosts = true;
  }

  subscribe(username: string) {
    this.optimisticFollow(true);

    this.userRest.PutSubscribers(username).subscribe({
      next: () => this.userCache.AddSubscriber(1),
      error: () => this.optimisticFollow(false)
    });
  }

  unsubscribe(username: string) {
    this.optimisticFollow(false);

    this.userRest.DeleteSubscribers(username).subscribe({
      next: () => this.userCache.AddSubscriber(-1),
      error: () => this.optimisticFollow(true)
    });
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
