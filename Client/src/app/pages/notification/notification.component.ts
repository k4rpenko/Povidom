import { Component } from '@angular/core';
import {HEADERComponent} from "../../components/main/header/header.component";
import {BorderMainComponent} from '../../components/main/border-main/border-main.component';

@Component({
  selector: 'app-notification',
    imports: [
        HEADERComponent,
        BorderMainComponent,
    ],
  templateUrl: './notification.component.html',
  styleUrl: './notification.component.scss'
})
export class NotificationComponent {


  
}
