import { Component, inject, Renderer2 } from '@angular/core';
import { BorderMainComponent } from "../../../components/border-main/border-main.component";
import { HEADERComponent } from "../../../components/header/header.component";
import { Post, Comment } from '../../../data/interface/Post/Post.interface';
import { PostService } from '../../../api/REST/post/Post.service';
import { PostCacheService } from '../../../data/cache/post.service';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { UserCacheService } from '../../../data/cache/user.service';
import { HomeComponent } from '../../home/home.component';
import { UserProfil } from '../../../data/interface/Users/UserProfil.interface';

@Component({
  selector: 'app-post',
  imports: [BorderMainComponent, CommonModule, HEADERComponent, RouterModule],
  templateUrl: './post.html',
  styleUrl: './post.scss',
})
export class PostID {
  Rest = inject(PostService);
  MaxHeight: number = 380;
  previousLength: number = 0;
  PrintLenght: number = 300;
  MaxLenghtText: number = 300;
  circleProgress: number = 100;
  circleColor: string = '#4caf50';
  loading: boolean = true;
  post!: Post;
  user!: UserProfil
  Coment: Post = {
    content: '',
    userId: "0",
    likeAmount: 0,
    commentAmount: 0,
    repostAmount: 0,
    viewsAmount: 0,
    shaveAnswer: false,
    ansver: null as any
  };

  constructor(
    private ActivatedRouter: ActivatedRoute, 
    private postsService: PostService, 
    private postCache: PostCacheService, 
    private userCache: UserCacheService, 
    private renderer: Renderer2, 
  ) {}


  ngOnInit() {
    const post_id = this.ActivatedRouter.snapshot.paramMap.get('id')?.toString()!;
    this.GetPost(post_id);

    var res = this.userCache.loadUser();
    res.subscribe(user => {
      this.user = user;
    });
  }

  navigateToPost(id: string) {
    console.log(1);
  }

  navigateToPostreply(id: string) {
    console.log(2);
  }


  likePost(id: string) {
    this.Rest.LikePost(id).subscribe({
      next: () => {
        this.post.likeAmount = (this.post?.likeAmount || 0) + 1;
        this.post.youLike = true;

        this.postCache.changPost(this.post);
      },
      error: (error) => {
        console.error(error);
      }
    });
  }

  DeleteLikePost(id: string) {
    this.Rest.DeleteLikePost(id).subscribe({
      next: () => {
        this.post.likeAmount = (this.post?.likeAmount || 0) - 1;
        this.post.youLike = false;

        this.postCache.changPost(this.post);
      },
      error: (error) => {
        console.error(error);
      }
    });
  }

  
  likeComent(id: string, id_coment: string) {
    this.Rest.LikeComent(id, id_coment).subscribe({
      next: () => {
        let coment = this.post.comments?.find(u => u.id === id_coment)!;
        coment.likeAmount++;
        coment.youLike = true;

        this.postCache.changPost(this.post);
      },
      error: (error) => {
        console.error(error);
      }
    });
  }

  DeletelikeComent(id: string, id_coment: string) {
    this.Rest.DeleteLikeComent(id, id_coment).subscribe({
      next: () => {
        let coment = this.post.comments?.find(u => u.id === id_coment)!;
        coment.likeAmount--;
        coment.youLike = false;

        this.postCache.changPost(this.post);
      },
      error: (error) => {
        console.error(error);
      }
    });
  }

  GetPost(id: string){
    this.Rest.GetPostById(id).subscribe({
      next: (res) => {
        this.post = res.post;
        this.loading = false;
      },
      error: (error) => {
        console.error(error);
      }
    });
  }

  autoResize(event: Event) {
    const el = event.target as HTMLElement;
    const text = el.innerText;
    this.Coment.content = el.innerText;
    //const diff = text.length - this.previousLength;
    
    this.PrintLenght = this.MaxLenghtText - text.length;
    if(text == "\n"){this.PrintLenght = 300;}
    

    this.previousLength = text.length;
    this.updateCircle();

    el.style.height = 'auto';

    if(el.scrollHeight >= 400){
      el.style.height = this.PrintLenght + 'px';
      el.style.overflowY = 'auto';
    }
    else {
      el.style.height = `${el.scrollHeight}px`;
      el.style.overflowY = 'hidden';
    }

    if (text.length > 300) {
      const normalPart = text.slice(0, 300);
      const excessPart = text.slice(300);

      this.renderer.setProperty(el, 'innerHTML', '');
      
      const normalNode = this.renderer.createText(normalPart);
      this.renderer.appendChild(el, normalNode);
      
      const span = this.renderer.createElement('span');
      this.renderer.addClass(span, 'overflow');
      const excessNode = this.renderer.createText(excessPart);
      this.renderer.appendChild(span, excessNode);
      this.renderer.appendChild(el, span);

      this.placeCursorAtEnd(el);
    }
  }

  placeCursorAtEnd(element: HTMLElement) {
    const range = document.createRange();
    const sel = window.getSelection();
    
    range.selectNodeContents(element);
    range.collapse(false);

    sel?.removeAllRanges();
    sel?.addRange(range);
  }

  updateCircle() {
    const max = 300;
    const percent = (this.PrintLenght / max) * 100;
    this.circleProgress = percent;

    if (this.PrintLenght <= 0) {
      this.circleColor = 'red';
    } else if (this.PrintLenght <= 30) {
      this.circleColor = 'yellow';
    } else {
      this.circleColor = 'green';
    }
  }

  SendComents() {
    this.Coment.id = this.post.id
    this.Coment.userId = this.user.id!

    this.post.comments?.push({
      userId: this.user.id,
      content: this.Coment.content,
      createdAt: new Date().toISOString(),
      user: this.user,
      repostAmount: 0,
      likeAmount: 0
    });

    this.Rest.AddComents(this.Coment).subscribe({
      next: () => {
        this.post.commentAmount = (this.post?.likeAmount || 0) + 1;
        this.post.youComment = true; 
        this.postCache.changPost(this.post);
        this.Coment.content = ""
      },
      error: (error) => {
        console.error(error);
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
