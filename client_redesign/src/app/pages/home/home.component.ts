import { Component } from '@angular/core';
import {HEADERComponent} from '../../components/header/header.component';
import {BorderMainComponent} from '../../components/border-main/border-main.component';
import {IndexDBComponent} from '../../api/UserDB/index-db/index-db.component';

@Component({
  selector: 'app-home',
  imports: [
    HEADERComponent,
    BorderMainComponent,
    IndexDBComponent
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {

  navigateToPost = () =>{
    console.log(1)
  }

  navigateToPostreply = () =>{
    console.log(2)
  }

}
