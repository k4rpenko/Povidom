import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AddPostComponent } from "../add-post/add-post.component";
import { UserCacheService } from '../../data/cache/user.service';
import { UserProfil } from '../../data/interface/Users/UserProfil.interface';
import { Observable, of } from 'rxjs';
import { RouterModule } from '@angular/router';
import { AuthGuard } from '../../guards/auth.guard';


@Component({
  selector: 'app-header',
  imports: [CommonModule, AddPostComponent, RouterModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HEADERComponent implements OnInit {
  @Input() url: string = '';

  user$!: Observable<UserProfil | null>;
  TOKEN!: Observable<boolean>;

  constructor(
    private userCache: UserCacheService,
    private guards: AuthGuard
  ) {}
  
  ngOnInit() { 
    this.TOKEN = this.guards.token.asObservable();
    console.log(this.TOKEN);
    
    this.user$ = this.userCache.user$;
    this.userCache.loadUser().subscribe();
  }


  isAddPostOpen = false;

  openAddPost() {
    this.isAddPostOpen = true;
  }

  closeAddPost() {
    this.isAddPostOpen = false;
  }

}
