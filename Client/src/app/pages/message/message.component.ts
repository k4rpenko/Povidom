import { Component, OnDestroy, OnInit } from "@angular/core";
import * as signalR from "@microsoft/signalr";
import { CheckUser } from "../../data/Global";
import { MatDialog } from "@angular/material/dialog";
import { FindPeopleComponent } from "../../content/Main/find-people/find-people.component";

@Component({
  selector: 'app-message',
  standalone: true,
  imports: [],
  templateUrl: './message.component.html',
  styleUrl: './message.component.scss'
})
export class MessageComponent implements OnInit, OnDestroy{
  private hubConnection: signalR.HubConnection | undefined;

  constructor(public dialog: MatDialog) {
    
  }
  


  ngOnInit(): void {
    this.startConnection();
  }

  ngOnDestroy(): void {
    this.stopConnection();
  }

  private startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${CheckUser.url}/message`)
      .configureLogging(signalR.LogLevel.Information)
      .build();


    this.hubConnection
      .start()
      .then(() => {
        console.log('Connection started');
        this.hubConnection?.invoke('Connect', '').then(result => {
          console.log('Connect result:', result);
      });
      })
      .catch(err => console.log('Error while starting connection: ' + err));
  }

  private stopConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop()
        .then(() => console.log('Connection stopped'))
        .catch(err => console.log('Error while stopping connection: ' + err));
    }
  }


  openFindPeopleComponent(): void {
    this.dialog.open(FindPeopleComponent, {});
  }
}
