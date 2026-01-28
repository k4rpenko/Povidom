import { Component, OnInit } from '@angular/core';
import {HEADERComponent} from '../../components/header/header.component';
import {BorderMainComponent} from '../../components/border-main/border-main.component';
import { UserProfil } from '../../data/interface/Users/UserProfil.interface';
import {CommonModule } from '@angular/common';
import { UserCacheService } from '../../data/cache/user.service';

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
  
  constructor( private userCache: UserCacheService, ) {}

  ngOnInit() {
    var res = this.userCache.loadUser();
    res.subscribe(user => {
      this.user = user;
    });
  }

  navigateToPost = () =>{
    console.log(1)
  }

  navigateToPostreply = () =>{
    console.log(2)
  }
}
