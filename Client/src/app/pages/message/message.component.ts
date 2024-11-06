import { CookieService } from "ngx-cookie-service";
import { WebSocketService } from "../../data/HTTP/WEB/WebSocket.service";
import { MatDialog } from "@angular/material/dialog";
import { Chats } from "../../data/interface/Chats/User.interface";
import { CommonModule } from "@angular/common";
import { Component, OnDestroy, OnInit } from "@angular/core";
import { FindPeopleComponent } from "../../content/Main/find-people/find-people.component";

@Component({
  selector: 'app-message',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './message.component.html',
  styleUrls: ['./message.component.scss']
})
export class MessageComponent implements OnInit, OnDestroy {
  public userStatus: { [userId: string]: boolean } = WebSocketService.userStatus;
  public Chats: Chats[] = WebSocketService.Chats;
  private id: string;

  constructor(
    public dialog: MatDialog,
    private cookieService: CookieService,
    private WS: WebSocketService
  ) {
    this.id = this.cookieService.get('authToken');
  }

  async ngOnInit(): Promise<void> {
    try {
      await this.WS.startConnection();
      this.WS.addListeners();
      await this.WS.GetChats(this.id);
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
