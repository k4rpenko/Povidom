import { Component } from '@angular/core';
import {HEADERComponent} from '../../components/main/header/header.component';
import {IndexDBComponent} from "../../api/UserDB/index-db/index-db.component";
import { Chat } from "../../components/messages/chat/chat";
import { Users } from "../../components/messages/users/users";
import { Chats } from '../../data/interface/Chats/Chats.interface';
import { UserProfil } from '../../data/interface/Users/UserProfil.interface';
import { SignalRService } from '../../api/WebSocket/SignalR.service';
import { Message } from '../../data/interface/Chats/Message.interface';

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
  public chat: Chats = {} as Chats;
  message: Message[] = [];
  
  constructor(private signalRService: SignalRService) {}
  
  getChat(userId: string) {
    this.signalRService.sendMessage("GetMessage", this.chat.id)
      .then((chat) => {
        this.chat = chat;
      })
      .catch(err => console.error(err));
  }

  public onUserSelected(user: UserProfil) {
    if (!this.chat) {
      this.chat = {} as Chats;
    }
    this.getChat(user.id!);
    this.chat.user = user;
  }
}
