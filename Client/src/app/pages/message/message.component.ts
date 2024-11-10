import { CookieService } from "ngx-cookie-service";
import { WebSocketService } from "../../data/HTTP/WEB/WebSocket.service";
import { MatDialog } from "@angular/material/dialog";
import { Chats } from "../../data/interface/Chats/User.interface";
import { CommonModule } from "@angular/common";
import { Component, OnDestroy, OnInit } from "@angular/core";
import { FindPeopleComponent } from "../../content/Main/find-people/find-people.component";
import { StatusModel } from "../../data/interface/Chats/StatusModel.interface";
import { TokenModel } from "../../data/interface/Chats/TokenModel";
import { Message } from "../../data/interface/Chats/Message.interface";
import { ChatModel } from '../../data/interface/Chats/ChatModel';
import { GetMessageModel } from "../../data/interface/Chats/GetMessageModel.interface";
import { FormsModule } from "@angular/forms";

@Component({
  selector: 'app-message',
  standalone: true,
  imports: [CommonModule, FormsModule ],
  templateUrl: './message.component.html',
  styleUrls: ['./message.component.scss']
})
export class MessageComponent implements OnInit, OnDestroy {
[x: string]: any;
  public userStatus: StatusModel = WebSocketService.userStatus;
  public Chats: Chats[] = WebSocketService.Chats;
  public Message: Message[] = WebSocketService.Message;
  public OpenChat!: GetMessageModel;
  public YouID!: string;
  private id: string;
  open: boolean = false;
  status: boolean = false;
  public message: string = "";

  constructor(
    public dialog: MatDialog,
    private cookieService: CookieService,
    private WS: WebSocketService
  ) {
    this.id = this.cookieService.get('authToken');
  }

  async ngDoCheck() {
    if (WebSocketService.Chats !== this.Chats || WebSocketService.userStatus !== this.userStatus) {
      this.Chats = WebSocketService.Chats;
      this.Message = WebSocketService.Message;
      this.userStatus = WebSocketService.userStatus;
    }
  }

  async ngOnInit() {
    try {
      await this.WS.startConnection();
      const tokenModel: TokenModel = {
        token: this.id
      }
      setTimeout(async () => {
        await this.WS.GetChats(tokenModel);
        this.YouID = await this.WS.GetId(this.cookieService.get('authToken'));
      }, 500);
    } catch (error) {
      console.error('Error during initialization:', error);
    }
  }

  ngOnDestroy(): void {
    this.WS.stopConnection();
  }

  openFindPeopleComponent(): void {
    this.dialog.open(FindPeopleComponent, {});
  }


  sendMessage(){
    if(this.message != null){
      const ChatModel: ChatModel = {
        IdChat: this.OpenChat.chatId,
        CreatorId: this.id,
        Text: this.message
      }
      this.WS.SendMessage(ChatModel);
      this.message = '';
    }
  }


  async OpenMessage(Chats: Chats){
    const ChatModel: ChatModel = {
      IdChat: Chats.chatId,
      CreatorId: this.id
    }

    const messages = await this.WS.GetMessage(ChatModel);

    this.OpenChat = {
      avatar: Chats.avatar,
      nickName: Chats.nickName,
      createdAt: Chats.createdAt,
      chatId: Chats.chatId,
      lastMessage: Chats.lastMessage,
      message: messages
    }

    if(this.OpenChat.avatar != null){
      this.open = true;
      this.WS.hubConnection?.on("ReceiveMessage", (newMessage) => {
        this.OpenChat.message?.push(newMessage);
      });
    }
  }
}
