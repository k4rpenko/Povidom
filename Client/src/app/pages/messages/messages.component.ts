import { Component } from '@angular/core';
import {HEADERComponent} from '../../components/header/header.component';
import {IndexDBComponent} from "../../api/UserDB/index-db/index-db.component";
import { Chat } from "../../components/messages/chat/chat";
import { Users } from "../../components/messages/users/users";

@Component({
  selector: 'app-messages',
    imports: [
    HEADERComponent,
    IndexDBComponent,
    Chat,
    Users
],
  templateUrl: './messages.component.html',
  styleUrl: './messages.component.scss'
})
export class MessagesComponent {

}
