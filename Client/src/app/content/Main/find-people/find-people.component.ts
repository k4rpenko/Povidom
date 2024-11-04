import { Component, inject } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { FindUserData } from '../../../data/HTTP/GetPosts/User/FindUserData.service';
import { User, userG } from '../../../data/interface/Users/User.interface';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-find-people',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './find-people.component.html',
  styleUrl: './find-people.component.scss'
})
export class FindPeopleComponent {
  profileService = inject(FindUserData);
  constructor(public dialog: MatDialog, public dialogRef: MatDialogRef<FindPeopleComponent>){}
  User: User[] = [];

  onClose(): void {
    this.dialogRef.close();
  }

  onInputChange(event: Event): void {
    const nick = (event.target as HTMLInputElement).value;
    this.profileService.FindUserData(nick).subscribe({
      next: (response) => {
        this.User = response.user;
      },
      error: (err) => {
        console.error(err);
      }
    });
  }
}
