import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Chats } from '../../interface/Chats/User.interface';
import { CheckUser } from '../../Global';
import { ChatModel } from '../../interface/Chats/ChatModel';

@Injectable({
  providedIn: 'root'
})
export class WebSocketService {
  private hubConnection: signalR.HubConnection | undefined;
  public static Chats: Chats[] = [];
  public static userStatus: { [userId: string]: boolean } = {};

  public async startConnection(): Promise<void> {
    console.log("start");
    
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${CheckUser.url}/message`)
      .configureLogging(signalR.LogLevel.Information)
      .build();
  }
  


  public async CreatChat(chatModel: ChatModel) {
    if (this.hubConnection) {
      try {
        const response = await this.hubConnection.invoke('CreateChat', chatModel);
        WebSocketService.Chats.push(response);
      } catch (err) {
        console.error('Error invoking CreateChat:', err);
      }
    } else {
      console.error('Hub connection is not established.');
    }
  }
  
  public GetChats(id: string) {
    if (this.hubConnection) {
      this.hubConnection
        .start()
        .then(() => {
          return this.hubConnection?.invoke("Connect", id);
        })
        .then(result => {
          WebSocketService.Chats = result;
        })
        .catch(err => {
          console.error('Error while starting connection or invoking Connect:', err);
        });
    }
  }

  public addListeners() {
    this.hubConnection?.on('Connect', (userId: string, status: boolean) => {
      WebSocketService.userStatus[userId] = status;
    });
  }



  public stopConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop()
        .then(() => console.log('Connection stopped'))
        .catch(err => console.log('Error while stopping connection: ' + err));
    }
  }
}
