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
    // Логіку для створення нового посту
    console.log('Create Post button clicked!');

    //Можна перенаправити на сторінку або щоб відкривалось якесь вікно 
  }
}