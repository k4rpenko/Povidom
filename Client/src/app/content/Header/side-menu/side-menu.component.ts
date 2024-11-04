import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-side-menu',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './side-menu.component.html',
  styleUrls: ['./side-menu.component.scss'] // Виправлено на styleUrls
})
export class SideMenuComponent {
  currentUrl: string = '';
  massage: boolean = false;
  home: boolean = false;
  explore: boolean = false;
  notifications: boolean = false;

  constructor(private router: Router) {
    this.router.events.subscribe(() => {
      this.currentUrl = this.router.url;
      this.checkUrl();
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
