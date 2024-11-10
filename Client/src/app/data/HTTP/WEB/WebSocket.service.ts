import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Chats } from '../../interface/Chats/User.interface';
import { CheckUser } from '../../Global';
import { ChatModel } from '../../interface/Chats/ChatModel';
import { StatusModel } from '../../interface/Chats/StatusModel.interface';
import { TokenModel } from '../../interface/Chats/TokenModel';
import { Message } from '../../interface/Chats/Message.interface';

@Injectable({
  providedIn: 'root'
})
export class WebSocketService {
  public hubConnection: signalR.HubConnection | undefined;
  public static Chats: Chats[];
  public static Message: Message[];
  public static userStatus: StatusModel;
  constructor() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${CheckUser.url}/message`)  
      .build();
    
  }


  public async startConnection() {
    this.hubConnection!
      .start()
      .then(() => console.log('SignalR підключення успішно встановлено!'))
      .catch(err => console.log('Помилка підключення до SignalR: ', err));
  }
  
  public async SendMessage(chatModel: ChatModel) {
    if (this.hubConnection) {
      try {
        const response = await this.hubConnection.invoke('SendMessage', chatModel);
        if(response == false){
          console.error('Error Send');
        }
      } catch (err) {
        console.error('Error invoking:', err);
      }
    } else {
      console.error('Hub connection is not established.');
    }
  }

  public async GetSendMessage() {
    if (this.hubConnection) {
      try {
        const response = await  this.hubConnection.on("ReceiveMessage", (newMessage) => {
          return newMessage;
        });
      }
      catch (err) {
        console.error('Error invoking:', err);
      }
    }
  }



  public async CreatChat(chatModel: ChatModel) {
    if (this.hubConnection) {
      try {
        await this.hubConnection.invoke('CreateChat', chatModel);
      } catch (err) {
        console.error('Error invoking CreateChat:', err);
      }
    } else {
      console.error('Hub connection is not established.');
    }
  }

  public async GetMessage(chatModel: ChatModel) {
    if (this.hubConnection) {
      try {
        const response = await this.hubConnection.invoke('GetMessage', chatModel);
        return response;
      } catch (err) {
        console.error('Error invoking:', err);
      }
    } else {
      console.error('Hub connection is not established.');
    }
  }
  
  public async GetId(token: string) {
    if (this.hubConnection) {
      try {
        const response = await this.hubConnection.invoke('GetId', token);
        return response;
      } catch (err) {
        console.error('Error invoking:', err);
      }
    } else {
      console.error('Hub connection is not established.');
    }
  }

  public async GetChats(tokenModel: TokenModel) {
    if (this.hubConnection) {
      try {
        const response = await this.hubConnection.invoke("GetChats", tokenModel);
        WebSocketService.Chats = response;
      } catch (err) {
        console.error('Error invoking:', err);
      }
    } else {
      console.error('Hub connection is not established.');
    }
  }
  
  /*public GetChats(tokenModel: TokenModel) {

    if (this.hubConnection) {
      this.hubConnection
        .start()
        .then(async () => {
          return await this.hubConnection!.invoke("GetChats", tokenModel);
        })
        .then(result => {
            WebSocketService.Chats = result;
        })
        .catch(err => {
          console.error('Error while starting connection or invoking Connect:', err);
        });
    } else {
      console.error('Hub connection is not established.');
    }
  }*/
  

  public stopConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop()
        .then(() => console.log('Connection stopped'))
        .catch(err => console.log('Error while stopping connection: ' + err));
    }
  }
}
