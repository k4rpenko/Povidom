import { Component } from '@angular/core';
import {HEADERComponent} from '../../components/header/header.component';
import {BorderMainComponent} from '../../components/border-main/border-main.component';

@Component({
  selector: 'app-user',
  imports: [
    HEADERComponent,
    BorderMainComponent
  ],
  templateUrl: './user.component.html',
  styleUrl: './user.component.scss'
})
export class UserComponent {

  navigateToPost = () =>{
    console.log(1)
  }

  navigateToPostreply = () =>{
    console.log(2)
  }
}
