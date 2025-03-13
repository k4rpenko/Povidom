import { Component } from '@angular/core';
import {BorderMainComponent} from "../../components/border-main/border-main.component";
import {HEADERComponent} from "../../components/header/header.component";
import {CommonModule} from '@angular/common';

@Component({
  selector: 'app-settings',
  imports: [ CommonModule,  HEADERComponent ],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.scss'
})
export class SettingsComponent {
  Types: string = "";
  url: string = '';
  biography: string = '';
  author: string = '';
  username: string = '';

  onInput(event: Event, field: string) {
    const editableElement = event.target as HTMLElement;

    switch (field) {
      case 'url':
        this.url = editableElement.innerText;
        break;
      case 'biography':
        this.biography = editableElement.innerText;
        break;
      case 'author':
        this.author = editableElement.innerText;
        break;
      case 'username':
        this.username = editableElement.innerText;
        break;
    }
  }


  setType(value: string) {
    this.Types = value;
  }
}
