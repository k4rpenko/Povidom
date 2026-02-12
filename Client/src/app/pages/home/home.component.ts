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

  ngOnInit() {
    if (this.posts.length === 0) {
      this.getPostsForYou();
    }
  }

  clean(){
    this.posts = [];
    this.loading = true;
  }

  setTypePost(i: number){
    this.TypePost = i;
    this.clean();
    if (this.TypePost === 1) {
      this.getPostsForYou();
    } else if (this.TypePost === 2) {
      this.getPostsFollowing();
    }
  }

  getPostsForYou() {
    this.postCache.loadPosts();
    this.postCache.postsSubject.subscribe(posts => {
      if(posts.length === 0) return;
      this.posts = posts;
      this.loading = false;
    });
  }

  getPostsFollowing() {
    this.postCache.loadFollowingPosts();
    this.postCache.followingPostsSubject.subscribe(posts => {
      if(posts.length === 0) return;
      this.posts = posts;
      this.loading = false;
    });
  }

  sendPost(post: Post){
    this.postCache.SendPost(post);
    this.postCache.postsSubject.subscribe(posts => { this.posts = posts });
  }
}