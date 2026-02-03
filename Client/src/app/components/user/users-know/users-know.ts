import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { UserProfil } from '../../../data/interface/Users/UserProfil.interface';
import { CommonModule } from '@angular/common';
import { UserREST } from '../../../api/REST/user/UserData.service';

@Component({
  selector: 'app-users-know',
  imports: [CommonModule],
  templateUrl: './users-know.html',
  styleUrl: './users-know.scss',
})
export class UsersKnow {
  @Output() closed = new EventEmitter<void>();
  @Input() user!: UserProfil
  @Input() type!: number
  users: UserProfil[] = []
  UserRest = inject(UserREST);

  constructor() {}

  ngOnInit() {
    if(this.type == 1){
      this.UserRest.GetFolowers(this.user.userName!).subscribe({
        next: (res) => {
          this.users = res.users;
        },
        error: (error)=>{
          close();
        }
      })
    }
    else if(this.type == 2){
      this.UserRest.GetSubscribers(this.user.userName!).subscribe({
        next: (res) => {
          this.users = res.users;
        },
        error: (error)=>{
          close();
        }
      })
    }
    else{
      close();
    }
  }

  close() {
    this.closed.emit();
  }
}
