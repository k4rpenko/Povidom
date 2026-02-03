import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AddPostComponent } from "../add-post/add-post.component";
import { UserCacheService } from '../../data/cache/user.service';
import { UserProfil } from '../../data/interface/Users/UserProfil.interface';
import { Observable, of } from 'rxjs';
import { RouterModule } from '@angular/router';


@Component({
  selector: 'app-header',
  imports: [CommonModule, AddPostComponent, RouterModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HEADERComponent implements OnInit {
  @Input() url: string = '';

  user$!: Observable<UserProfil>;
  user!: UserProfil;

  constructor(
    private userCache: UserCacheService,
  ) {}
  
  ngOnInit() { 
    var res = this.userCache.loadUser();
    res.subscribe(user => {
      this.user = user;
    });
  }


  isAddPostOpen = false;

  openAddPost() {
    this.isAddPostOpen = true;
  }

  closeAddPost() {
    this.isAddPostOpen = false;
  }

}
