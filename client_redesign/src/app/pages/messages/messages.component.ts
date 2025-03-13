import { Component } from '@angular/core';
import {HEADERComponent} from '../../components/header/header.component';
import {IndexDBComponent} from "../../api/UserDB/index-db/index-db.component";

@Component({
  selector: 'app-messages',
    imports: [
        HEADERComponent,
        IndexDBComponent
    ],
  templateUrl: './messages.component.html',
  styleUrl: './messages.component.scss'
})
export class MessagesComponent {

}
