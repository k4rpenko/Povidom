import { Component, HostListener, inject, OnInit } from '@angular/core';
import { HEADERComponent } from '../../components/header/header.component';
import { BorderMainComponent } from '../../components/border-main/border-main.component';
import { Post } from '../../data/interface/Post/Post.interface';
import { PostService } from '../../api/REST/post/Post.service';
import { PostCacheService } from '../../data/cache/post.service';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { PostComponent } from "../../components/post/post";

@Component({
  selector: 'app-home',
  imports: [
    HEADERComponent,
    CommonModule,
    BorderMainComponent,
    RouterModule,
    PostComponent
],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  TypePost: number = 1;
  Rest = inject(PostService);
  openedRepostMenuId: string | null = null;
  MessageText: string = "";
  MessageAction: boolean = false;
  loading: boolean = true;
  
  constructor(public postCache: PostCacheService) {}

  posts: Post[] = [];
  ForYouPosts: Post[] = [];
  FollowingPosts: Post[] = [];

  async ngOnInit() {
    await this.getPostsForYou();

    this.postCache.postsSubject.subscribe(posts => {
      this.ForYouPosts = posts;
      if (this.TypePost === 1) this.posts = [...this.ForYouPosts];
      this.loading = false;
    });

    this.postCache.followingPostsSubject.subscribe(posts => {
      this.FollowingPosts = posts;
      if (this.TypePost === 2) this.posts = [...this.FollowingPosts];
      this.loading = false;
    });
  }

  clean(){
    this.posts = [];
    this.loading = true;
  }

  async setTypePost(i: number) {
    if (this.TypePost === i) return;

    this.TypePost = i;
    this.loading = true;

    if (i === 1) {
      if (this.postCache.isLoaded) {
        await this.getPostsForYou();
      } else {
        this.posts = [...this.ForYouPosts];
        this.loading = false;
      }
    }

    if (i === 2) {
      if (this.postCache.isFollowingLoaded) {
        await this.getPostsFollowing();
      } else {
        this.posts = [...this.FollowingPosts];
        this.loading = false;
      }
    }
  }

  async getPostsForYou() {
    this.loading = true;
    await this.postCache.loadPosts();
  }

  async getPostsFollowing() {
    this.loading = true;
    await this.postCache.loadFollowingPosts();
  }

  sendPost(post: Post){
    this.postCache.SendPost(post);
    this.postCache.postsSubject.subscribe(posts => { this.posts = posts });
  }
}