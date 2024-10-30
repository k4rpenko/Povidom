import { Component } from '@angular/core';

@Component({
  selector: 'app-side-menu',
  standalone: true,
  imports: [],
  templateUrl: './side-menu.component.html',
  styleUrl: './side-menu.component.scss'
})
export class SideMenuComponent {

  constructor() { }

  createPost() {
    console.log('Create Post button clicked!');
  }
}