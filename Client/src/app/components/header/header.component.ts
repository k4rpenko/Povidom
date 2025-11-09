import {Component, Input} from '@angular/core';
import {CommonModule} from '@angular/common';
import { AddPostComponent } from "../../modals/add-post/add-post.component";

@Component({
  selector: 'app-header',
  imports: [CommonModule, AddPostComponent],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HEADERComponent {
  @Input() url: string = '';

  isAddPostOpen = false;

  openAddPost() {
    console.log("TEST");
    
    this.isAddPostOpen = true;
  }

  closeAddPost() {
    this.isAddPostOpen = false;
  }

}
