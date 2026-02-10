import { Component, EventEmitter, inject, Input, Output, Renderer2 } from '@angular/core';
import { UserProfil } from '../../data/interface/Users/UserProfil.interface';
import { UserCacheService } from '../../data/cache/user.service';
import { Post } from '../../data/interface/Post/Post.interface';
import { Router } from '@angular/router';
import { PostService } from '../../api/REST/post/Post.service';
import { PostCacheService } from '../../data/cache/post.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-add-post',
  imports: [CommonModule],
  templateUrl: './add-post.component.html',
  styleUrl: './add-post.component.scss'
})
export class AddPostComponent {
  @Output() closed = new EventEmitter<void>();
  @Input({ required: true }) type!: string;
  @Input() postQuote?: Post;

  Rest = inject(PostService);
  user!: UserProfil

  post: Post = {
    content: '',
    userId: "0",
    likeAmount: 0,
    commentAmount: 0,
    repostAmount: 0,
    viewsAmount: 0,
  };

  QuotePost: Post = {
    content: '',
    userId: "0",
    likeAmount: 0,
    commentAmount: 0,
    repostAmount: 0,
    viewsAmount: 0,
    shaveAnswer: true,
    ansver: this.postQuote
  };


  MaxHeight: number = 380;
  previousLength: number = 0;
  PrintLenght: number = 300;
  MaxLenghtText: number = 300;
  circleProgress: number = 100;
  circleColor: string = '#4caf50';

  constructor( 
    private userCache: UserCacheService, 
    private renderer: Renderer2, 
    private router: Router,
    private postCache: PostCacheService
  ) {}

  ngOnInit() {
    console.log(this.postQuote);
    
    var res = this.userCache.loadUser();
    res.subscribe(user => {
      if(user != null){
        this.user = user;
      }
    });

    this.userCache.getUser()
      .subscribe(user => {
        if (!user) return;
        this.user = user;
    });
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


  autoResize(event: Event) {
    const el = event.target as HTMLElement;
    const text = el.innerText;
    this.post.content = el.innerText;
    //const diff = text.length - this.previousLength;
    
    this.PrintLenght = this.MaxLenghtText - text.length;
    if(text == "\n"){this.PrintLenght = 300;}
    

    this.previousLength = text.length;
    this.updateCircle();

    el.style.height = 'auto';

    if (el.scrollHeight >= this.MaxHeight) {
      el.style.height = this.MaxHeight + 'px';
      el.style.overflowY = 'auto';
    } else {
      el.style.height = el.scrollHeight + 'px';
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

  SendPost(){
    this.Rest.AddPost(this.post).subscribe({
      next: (response) => {
        this.postCache.SendPost(response.post);
        this.router.navigate(['/home'])
      },
      error: (error) => {
        console.log(error);
      }
    });

    this.close();
  }

  close() {
    this.closed.emit();
  }
}