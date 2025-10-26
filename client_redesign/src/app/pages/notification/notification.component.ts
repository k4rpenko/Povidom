import { Component } from '@angular/core';
import {HEADERComponent} from "../../components/header/header.component";
import {BorderMainComponent} from '../../components/border-main/border-main.component';
import {IndexDBComponent} from "../../api/UserDB/index-db/index-db.component";

@Component({
  selector: 'app-notification',
    imports: [
        HEADERComponent,
        BorderMainComponent,
        IndexDBComponent
    ],
  templateUrl: './notification.component.html',
  styleUrl: './notification.component.scss'
})
export class NotificationComponent {


  
}
