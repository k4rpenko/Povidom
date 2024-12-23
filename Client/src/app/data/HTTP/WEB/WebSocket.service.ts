import { inject, Injectable, OnInit } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { CheckUser } from '../../Global';
import { User } from '../../interface/Users/AllDataUser.interface';;
import { MemoryCacheService } from '../../Cache/MemoryCacheService';
import { UserChangGet } from '../GetPosts/User/UserChangGet.service';
import { Chats } from '../../interface/Chats/Chats.interface';

@Injectable({
  providedIn: 'root'
})
export class WebSocketService {
  public hubConnection: signalR.HubConnection | undefined;
  private UserData!: User;
  UserChangGet = inject(UserChangGet);

  constructor(private cache: MemoryCacheService) { this.initializeService();}

  async initializeService() {
    await this.loadUserData();
    if (this.UserData?.id) {
      this.hubConnection = new signalR.HubConnectionBuilder()
        .withUrl(`${CheckUser.url}/message`)
        .withAutomaticReconnect()
        .build();
    }

  }

  public async Connect(token: string) {
    if (this.hubConnection) {
      const response = await this.hubConnection.invoke('Connect', token);
      return response;
    }
    return false;
  }


  private async loadUserData() {
    try {
      const result = await this.cache.getItem('User');
      if (result == null) {
        await this.AddCacheUser();
      } else {
        this.UserData = result;
      }
    } catch (error) {
      console.error('Error fetching user data:', error);
    }
  }

  private AddCacheUser() {
    return this.UserChangGet.Get().subscribe(response => {
      this.UserData = response.user;
      this.cache.setItem("User", this.UserData);
    });
  }

  public async startConnection(): Promise<void> {
    try {
      await this.hubConnection?.start();
    } catch (error) {
      alert(`Помилка підключення до SignalR: ${error}`);
    }
  }

  public async SendMessage(Chats: Chats) {
    if (this.hubConnection) {
      try {
        const response = await this.hubConnection.invoke('SendMessage', Chats);
        return response
      } catch (err) {
        console.error('Error invoking:', err);
      }
    } else {
      console.error('Hub connection is not established.');
    }
  }

  onReceiveMessage(callback: (Chats: Chats) => void) {
    if (this.hubConnection) {
      this.hubConnection.on("ReceiveMessage", (Chats: Chats) => {
        callback(Chats);
      });
    } else {
      console.error("Hub connection is not established.");
    }
  }





  public async CreatChat(Chats: Chats) {
    if (this.hubConnection) {
      try {
        await this.hubConnection.invoke('CreateChat', Chats);
      } catch (err) {
        console.error('Error invoking CreateChat:', err);
      }
    } else {
      console.error('Hub connection is not established.');
    }
  }

  public async GetMessage(Chats: Chats) {
    if (this.hubConnection) {
      try {
        const response = await this.hubConnection.invoke('GetMessage', Chats);
        return response;
      } catch (err) {
        console.error('Error invoking:', err);
      }
    } else {
      console.error('Hub connection is not established.');
    }
  }

  public async GetId() {
    return this.UserData.id!
  }

  public async GetChats(token: string) {
    if (this.hubConnection) {
      try {
        const response = await this.hubConnection.invoke("GetChats", token);
        return response
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

    public async View(Chats: Chats){
      if (this.hubConnection) {
        try {
          const result = await this.hubConnection.invoke("ViewMessage", Chats);
          return result;
        } catch (err) {
          console.error('Error invoking ViewMessage:', err);
        }
      }
    }



  GetView(callback: (chatId: string) => void) {
    if (this.hubConnection) {
      this.hubConnection.on("ViewMessage", (chatId: string) => {
        callback(chatId);
      });
    } else {
      console.error("Hub connection is not established.");
    }
  }

  GetStatusUser(callback: (idUser: string, status: boolean) => void) {
    if (this.hubConnection) {
      this.hubConnection.on("StatusUser", (idUser: string, status: boolean) => {
        callback(idUser, status);
      });
    } else {
      console.error("Hub connection is not established.");
    }
  }

  public async Update(token: string) {
    if (this.hubConnection) {
      try {
        await this.hubConnection.invoke("Update", token);
      } catch (err) {
        console.error('Error invoking ViewMessage:', err);
      }
    }
  }

  public delay(ms: number): Promise<void> {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  public async UpdateConnect(token: string) {
    while (true) {
      await this.delay(5000);
      if (this.hubConnection) {
        await this.hubConnection.invoke('Update', token);
      }
    }
  }
}
