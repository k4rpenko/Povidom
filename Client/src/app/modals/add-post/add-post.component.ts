import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-add-post',
  imports: [],
  templateUrl: './add-post.component.html',
  styleUrl: './add-post.component.scss'
})
export class AddPostComponent {
  @Output() closed = new EventEmitter<void>();

  close() {
    this.closed.emit();
  }
}