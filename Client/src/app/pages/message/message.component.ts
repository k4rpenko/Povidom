import { CookieService } from "ngx-cookie-service";
import { WebSocketService } from "../../data/HTTP/WEB/WebSocket.service";
import { MatDialog } from "@angular/material/dialog";
import { Chats } from "../../data/interface/Chats/User.interface";
import { CommonModule } from "@angular/common";
import { Component, OnDestroy, OnInit } from "@angular/core";
import { FindPeopleComponent } from "../../content/Main/find-people/find-people.component";
import { StatusModel } from "../../data/interface/Chats/StatusModel.interface";
import { TokenModel } from "../../data/interface/Chats/TokenModel";
import { Message, MessageModel } from "../../data/interface/Chats/Message.interface";
import { SendModel} from "../../data/interface/Chats/SendModel.interface";
import { ChatModel } from '../../data/interface/Chats/ChatModel';
import { CharsModel } from "../../data/interface/Chats/CharsModel.interface";
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
  public userStatus!: StatusModel;
  public Chats!: Chats[];
  public Message!: MessageModel[];
  public OpenChat: CharsModel = {} as CharsModel;
  public YouID!: string;
  private id: string;
  public open: boolean = false;
  status: boolean = false;
  public message: string = "";

  constructor(
    public dialog: MatDialog,
    private cookieService: CookieService,
    private WS: WebSocketService
  ) {
    this.id = this.cookieService.get('authToken');
  }

  async ngOnInit() {
    try {
      await this.WS.startConnection();
      var connect = await this.WS.Connect(this.id);
      if(!connect){
        this.WS.stopConnection(this.id);
      }
      else{
        this.Chats = await this.WS.GetChats(this.id);
        this.YouID = await this.WS.GetId(this.cookieService.get('authToken'));
        await this.WS.Connect(this.id);
        this.WS.onReceiveMessage((Message: MessageModel) => {
          this.updateMessage(Message);
        });
        this.WS.GetView((chatId: string) => {
          var chats = this.Chats.filter(u => u.chatId == chatId)
          chats.forEach(element => {
            element.view = true;
          });
        });
      }
    } catch (error) {
      console.error('Error during initialization:', error);
    }
  }

  ngOnDestroy(): void {
    this.WS.stopConnection(this.id);
  }

  openFindPeopleComponent(): void {
    this.dialog.open(FindPeopleComponent, {});
  }



  async sendMessage(){
    if(this.message != null && this.message != ""){
      const ChatModel: ChatModel = {
        IdChat: this.OpenChat.chatId,
        CreatorId: this.id,
        Text: this.message,
        CreatedAt: new Date(),
      };
      var idmessage = this.OpenChat.message![this.OpenChat.message!.length - 1].id + 1;

      const MessageModel: Message = {
        id: idmessage,
        idUser: this.YouID, 
        text: this.message,
        view: false,
        send: false,
        createdAt: ChatModel.CreatedAt!
      };
      
      const AddChat = this.Chats.find(u => u.chatId === ChatModel.IdChat);
      if (AddChat) {
        AddChat.lastMessage = {
          message: this.message,
          userId: this.YouID
        };
        this.OpenChat.message?.push(MessageModel);
        var statusMessage = await this.WS.SendMessage(ChatModel);
        if(statusMessage){
          this.OpenChat.message![this.OpenChat.message!.length - 1].send = true;
        }
      }

      this.message = '';
    }
  }


  async OpenMessage(Chats: Chats){
    const messages = await this.WS.GetMessage({ IdChat: Chats.chatId, CreatorId: this.id });
    
    this.OpenChat = {} as CharsModel;
    this.OpenChat = {
      avatar: Chats.avatar,
      nickName: Chats.nickName,
      createdAt: Chats.createdAt,
      chatId: Chats.chatId,
      lastMessage: Chats.lastMessage,
      message: messages
    }
    
    if (this.OpenChat.chatId != null) {
      this.open = true;
    }
  }


  
  async updateMessage(Message: MessageModel) {
    if (this.OpenChat.chatId === Message.idChat) {
      if (Message.message.idUser !== this.YouID) {
        Message.message.view = true;
        const send: SendModel = {
          idChat: Message.idChat,
          creatorId: Message.message.idUser
        };    
        await this.WS.View(send);
        this.OpenChat.message?.push(Message.message);
      }
    }
  
    const chat = this.Chats.find(u => u.chatId === Message.idChat);
    if (chat != null) {
      chat.lastMessage.message = Message.message.text;
      chat.lastMessage.userId = Message.message.idUser;
    }
  }




  handleKeydown(event: KeyboardEvent): void {
    if (event.key === 'Enter' && !event.shiftKey) {
      this.sendMessage();
      event.preventDefault();
    }
  }
}
