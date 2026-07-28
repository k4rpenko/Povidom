import { Component, Input } from '@angular/core';
import { Chats } from '../../../data/interface/Chats/Chats.interface';
import { CommonModule } from '@angular/common';
import { SignalRService  } from '../../../api/WebSocket/SignalR.service';
import { MessageModel } from '../../../data/interface/Chats/Message.interface';


@Component({
  selector: 'app-chat',
  imports: [CommonModule],
  templateUrl: './chat.html',
  styleUrl: './chat.scss',
})
export class Chat {
  @Input({ required: true }) chat!: Chats;

  constructor(private signalRService: SignalRService) {}
  

  sendMessage(value: string){
    const messageSend: MessageModel = { 
      idChat: this.chat.id!,
      peopleId: this.chat.user?.id,
      message: {
        text: value,
        yourMessage: true,
      }
     };
    this.signalRService.sendMessage("SendMessage", messageSend)
      .then((chat) => {
        this.chat = chat;
      })
      .catch(err => console.error(err));
  }
}
