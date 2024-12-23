import { Component, OnDestroy, OnInit, ViewEncapsulation } from "@angular/core";
import { CommonModule, } from "@angular/common";
import { MatDialog } from "@angular/material/dialog";
import { FormsModule } from "@angular/forms";

import { CookieService } from "ngx-cookie-service";
import { WebSocketService } from "../../data/HTTP/WEB/WebSocket.service";
import { FindPeopleComponent } from "../../floating/find-people/find-people.component";
import { Message, MessageModel } from "../../data/interface/Chats/Message.interface";
import { Chats } from "../../data/interface/Chats/Chats.interface";

@Component({
  selector: 'app-message',
  standalone: true,
  imports: [CommonModule, FormsModule ],
  templateUrl: './message.component.html',
  styleUrls: ['./message.component.scss'],
  encapsulation: ViewEncapsulation.Emulated
})
export class MessageComponent implements OnInit {
[x: string]: any;
  public Chats!: Chats[];
  public Message!: MessageModel[];
  public OpenChat: Chats = {} as Chats;
  public YouID!: string;
  private id: string;
  public open: boolean = false;
  status: boolean = false;
  public message: string = "";

  constructor(
    public dialog: MatDialog,
    private cookieService: CookieService,
    private WS: WebSocketService,
  ) {
    this.id = this.cookieService.get('authToken');
  }

  async ngOnInit() {
    try {
      await this.WS.startConnection();
      var connect = await this.WS.Connect(this.id);
      if(connect){
        this.Chats = await this.WS.GetChats(this.id);
        this.YouID = await this.WS.GetId();

        await this.WS.Connect(this.id);
        this.WS.GetStatusUser((idUser: string, status: boolean) => {
          var chats = this.Chats.filter(u => u.user?.id == idUser)
          chats.forEach(element => {
            element.user!.isOnline = status;
          });
          if(this.OpenChat.id){
            this.OpenChat.user!.isOnline = status;
          }
        });
        this.WS.onReceiveMessage((Chats: Chats) => {
          this.updateMessage(Chats);
        });
        this.WS.GetView((chatId: string) => {
          var chats = this.Chats.filter(u => u.id == chatId)
          chats.forEach(element => {
            element.message!.view = true;
          });
          var OpenChat = this.OpenChat.messageArray?.filter(u => u.idUser === this.YouID && u.view !== true);
          OpenChat!.forEach(element => {
            element.view = true;
          });
        });
        this.WS.UpdateConnect(this.id);
      }
    } catch (error) {
      console.error('Error during initialization:', error);
    }
  }

  openFindPeopleComponent(): void {
    this.dialog.open(FindPeopleComponent, {});
  }



  async sendMessage(){
    if(this.message != null && this.message != ""){
      const ChatModel: Chats = {
        id: this.OpenChat.id,
        user: {
          id: this.id,
        },
        message: {
          text: this.message,
          createdAt: new Date(),
        },

      };

      const MessageModel: Message = {
        idUser: this.YouID,
        text: this.message,
        view: false,
        send: false,
        createdAt: ChatModel.message?.createdAt!
      };

      const AddChat = this.Chats.find(u => u.id === ChatModel.id);
      if (AddChat) {
        AddChat.message = {
          text: this.message,
          idUser: this.YouID
        };
        this.OpenChat.messageArray?.push(MessageModel);
        var statusMessage = await this.WS.SendMessage(ChatModel);
        this.message = '';
        if(statusMessage > 0){
          this.OpenChat.messageArray![this.OpenChat.messageArray!.length - 1].send = true;
          MessageModel.id = statusMessage
        }
      }
    }
  }


  async OpenMessage(Chats: Chats){
    const Send: Chats = {
      id: Chats.id,
      user: {
        id: this.id
      }
    }
    const messages = await this.WS.GetMessage(Send);
    this.OpenChat = {} as Chats;
    this.OpenChat = {
      id: Chats.id,
      user:{
        id: Chats.user?.id,
        avatar: Chats.user!.avatar,
        userName: Chats.user!.userName,
        lastName: Chats.user!.lastName,
        isOnline: Chats.user!.isOnline,
      },
      messageArray: messages
    }

    if (this.OpenChat.id != null) {
      this.open = true;
      if(this.OpenChat.message?.idUser != this.YouID){
        const send: Chats = {
          id: this.OpenChat.id,
          user: {
            id: this.OpenChat.user!.id
          }
        };
        await this.WS.View(send);
        const chat = this.Chats.find(u => u.id === Chats.id);
        chat!.message!.view = true;
      }
    }
  }



  async updateMessage(Chats: Chats) {
    if (this.OpenChat.id === Chats.id) {
      if (this.YouID !== Chats.message?.idUser) {
        Chats.message!.view = true;
        const send: Chats = {
          id: Chats.id,
          user: {
            id: Chats.message!.idUser
          }
        };
        await this.WS.View(send);
        this.OpenChat.messageArray?.push(Chats.message!);
      }
    }

    const chat = this.Chats.find(u => u.id === Chats.id);
    if (chat != null) {
      chat.message!.text = Chats.message!.text;
      chat.message!.idUser = Chats.message!.idUser;
      chat.message!.view = Chats.message!.view;
    }
  }

  async UpdateStatus(Message: MessageModel) {
    setInterval(async() => {
        await this.WS.Update(this.id);
    }, 5000);
  }




  handleKeydown(event: KeyboardEvent): void {
    if (event.key === 'Enter' && !event.shiftKey) {
      this.sendMessage();
      event.preventDefault();
    }
  }
}
