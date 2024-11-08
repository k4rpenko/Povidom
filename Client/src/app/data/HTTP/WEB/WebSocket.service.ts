import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Chats } from '../../interface/Chats/User.interface';
import { CheckUser } from '../../Global';
import { ChatModel } from '../../interface/Chats/ChatModel';
import { StatusModel } from '../../interface/Chats/StatusModel.interface';
import { TokenModel } from '../../interface/Chats/TokenModel';

@Injectable({
  providedIn: 'root'
})
export class WebSocketService {
  private hubConnection: signalR.HubConnection | undefined;
  public static Chats: Chats[];
  public static userStatus: StatusModel;

  public async startConnection(): Promise<void> {
    console.log("start");
    
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${CheckUser.url}/message`)
      .configureLogging(signalR.LogLevel.Information)
      .build();
  }
  


  public async CreatChat(chatModel: ChatModel) {
    console.log(chatModel);
    
    if (this.hubConnection) {
      try {
        const response = await this.hubConnection.invoke('CreateChat', chatModel);
        if (WebSocketService.Chats) {
          WebSocketService.Chats.push(response);
        } else {
          WebSocketService.Chats = [response];
        }
      } catch (err) {
        console.error('Error invoking CreateChat:', err);
      }
    } else {
      console.error('Hub connection is not established.');
    }
  }
  
  
  public GetChats(tokenModel: TokenModel) {

    if (this.hubConnection) {
      this.hubConnection
        .start()
        .then(async () => {
          return await this.hubConnection!.invoke("GetChats", tokenModel);
        })
        .then(result => {
            console.log(result);
            WebSocketService.Chats = result;
        })
        .catch(err => {
          console.error('Error while starting connection or invoking Connect:', err);
        });
    } else {
      console.error('Hub connection is not established.');
    }
  }
  

  public stopConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop()
        .then(() => console.log('Connection stopped'))
        .catch(err => console.log('Error while stopping connection: ' + err));
    }
  }
}
