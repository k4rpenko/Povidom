import { CookieService } from "ngx-cookie-service";
import { WebSocketService } from "../../data/HTTP/WEB/WebSocket.service";
import { MatDialog } from "@angular/material/dialog";
import { Chats } from "../../data/interface/Chats/User.interface";
import { CommonModule } from "@angular/common";
import { Component, OnDestroy, OnInit } from "@angular/core";
import { FindPeopleComponent } from "../../content/Main/find-people/find-people.component";
import { StatusModel } from "../../data/interface/Chats/StatusModel.interface";
import { TokenModel } from "../../data/interface/Chats/TokenModel";

@Component({
  selector: 'app-message',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './message.component.html',
  styleUrls: ['./message.component.scss']
})
export class MessageComponent implements OnInit, OnDestroy {
  public userStatus: StatusModel = WebSocketService.userStatus;
  public Chats: Chats[] = WebSocketService.Chats;
  private id: string;

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
      this.userStatus = WebSocketService.userStatus;
    }
  }

  async ngOnInit() {
    try {
      await this.WS.startConnection();
      const tokenModel: TokenModel = {
        token: this.id
      }
      await this.WS.GetChats(tokenModel);

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
}
