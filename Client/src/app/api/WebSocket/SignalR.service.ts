import { Injectable } from "@angular/core";
import * as signalR from "@microsoft/signalr";
import { BehaviorSubject } from "rxjs";
import { Message } from "../../data/interface/Chats/Message.interface";

@Injectable({
  providedIn: 'root'
})
export class SignalRService {

  protected hubConnection!: signalR.HubConnection;
  private messagesSubject = new BehaviorSubject<any[]>([]);
  public messages$ = this.messagesSubject.asObservable();
  
  startConnection(token?: string) {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl("https://localhost:8087/message", {
        withCredentials: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.onreconnecting(error => {
      console.warn('SignalR reconnecting', error);
    });

    this.hubConnection.onreconnected(connectionId => {
      console.log('SignalR reconnected', connectionId);
      this.invokeConnect();
    });

    this.hubConnection.onclose(error => {
      console.error('SignalR connection closed', error);
    });

    this.hubConnection.start()
      .then(() => {
        console.log("SignalR connected");
        this.invokeConnect();
      })
      .catch(err => console.error("SignalR failed to connect:", err));
  }

  invokeConnect() {
    if (this.hubConnection.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.invoke("Connect")
        .then(result => console.log("Connect result:", result))
        .catch(err => console.error("Connect failed:", err));
    } else {
      console.warn("Cannot invoke Connect: hub not connected yet");
    }
  }

  sendMessage(method: string, payload: any) {
    if (this.hubConnection.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.invoke(method, payload)
        .catch(err => console.error(err));
    } else {
      console.error("HubConnection не підключено!");
    }
  }


  public getChatMessages(chatId: string): Promise<Message[]> {
    return this.hubConnection.invoke("GetMessage", { id: chatId });
  }

  public get connection(): signalR.HubConnection {
    return this.hubConnection;
  }
}