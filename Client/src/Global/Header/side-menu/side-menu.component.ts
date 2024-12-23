import { CommonModule } from '@angular/common';
import { Component, HostListener, inject } from '@angular/core';
import { Router } from '@angular/router';
import { MemoryCacheService } from '../../../app/data/Cache/MemoryCacheService';
import { UserChangGet } from '../../../app/data/HTTP/GetPosts/User/UserChangGet.service';
import { User } from '../../../app/data/interface/Users/AllDataUser.interface';

@Component({
  selector: 'app-side-menu',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './side-menu.component.html',
  styleUrls: ['./side-menu.component.scss']
})
export class SideMenuComponent {
  buttonText: string = '';
  UserChangGet = inject(UserChangGet);

  currentUrl: string = '';
  massage: boolean = false;
  home: boolean = false;
  explore: boolean = false;
  notifications: boolean = false;
  UserData!: User;

  constructor(private router: Router, private cache: MemoryCacheService) {
    this.router.events.subscribe(() => {
      this.currentUrl = this.router.url;
      this.checkUrl();
    });
    this.loadUserData();
    this.onResize();
  }

  @HostListener('window:resize', ['$event'])
  onResize() {
    if (window.innerWidth < 1300) {
      this.buttonText = 'S';
    } else {
      this.buttonText = 'Send Post';
    }
  }

  private async loadUserData() {
    try {
      const result = await this.cache.getItem('User');
      if (result == null) {
        await this.AddCacheUser();
      } else {
        this.UserData = result;
      }
    } catch (error) {
      console.error('Error fetching user data:', error);
    }
  }

  private AddCacheUser() {
    return this.UserChangGet.Get().subscribe(response => {
      this.UserData = response.user;
      this.cache.setItem("User", this.UserData);
    });
  }

  createPost() {

  }

  checkUrl() {
    this.massage = false;
    this.home = false;
    this.explore = false;
    this.notifications = false;


    switch (this.currentUrl) {
      case '/message':
        this.massage = true;
        break;
      case '/home':
        this.home = true;
        break;
      case '/explore':
        this.explore = true;
        break;
      case '/notifications':
        this.notifications = true;
        break;
    }
  }
}
