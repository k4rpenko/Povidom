import { Component, EventEmitter, inject, Input, Output, Renderer2 } from '@angular/core';
import { UserProfil } from '../../data/interface/Users/UserProfil.interface';
import { UserCacheService } from '../../data/cache/user.service';
import { Post } from '../../data/interface/Post/Post.interface';
import { Router } from '@angular/router';
import { PostService } from '../../api/REST/post/Post.service';
import { HomeComponent } from '../../pages/home/home.component';

@Component({
  selector: 'app-add-post',
  imports: [],
  templateUrl: './add-post.component.html',
  styleUrl: './add-post.component.scss'
})
export class AddPostComponent {
  @Output() closed = new EventEmitter<void>();
  @Input() user!: UserProfil
  Rest = inject(PostService);

  post: Post = {
    content: '',
    userId: "0",
    likeAmount: 0,
    commentAmount: 0,
    retpostAmount: 0,
    viewsAmount: 0,
    shaveAnswer: false,
    ansver: null as any
  };



  MaxHeight: number = 380;
  previousLength: number = 0;
  MaxLenghtText: number = 300;
  circleProgress: number = 100;
  circleColor: string = '#4caf50';

  constructor( private userCache: UserCacheService, private home: HomeComponent, private renderer: Renderer2, private router: Router) {}

  ngOnInit() {
    var res = this.userCache.loadUser();
    res.subscribe(user => {
      this.user = user;
    });
  }
  
  updateCircle() {
    const max = 300;
    const percent = (this.MaxLenghtText / max) * 100;
    this.circleProgress = percent;

    if (this.MaxLenghtText <= 0) {
      this.circleColor = 'red';
    } else if (this.MaxLenghtText <= 30) {
      this.circleColor = 'yellow';
    } else {
      this.circleColor = 'green';
    }
  }


  autoResize(event: Event) {
    const el = event.target as HTMLElement;
    const text = el.innerText;
    this.post.content = el.innerText;
    const diff = text.length - this.previousLength;

    
    
    this.MaxLenghtText -= diff;
    if(diff == 0){this.MaxLenghtText = 300}
    

    if (this.MaxLenghtText > 300) this.MaxLenghtText = 300;

    this.previousLength = text.length;
    this.updateCircle();

    el.style.height = 'auto';

    if(el.scrollHeight >= 400){
      el.style.height = this.MaxHeight + 'px';
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

  SendPost(){
    this.Rest.AddPost(this.post).subscribe({
      next: (response) => {
        this.home.sendPost(response.post);
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