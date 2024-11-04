import { Component } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-find-people',
  standalone: true,
  imports: [],
  templateUrl: './find-people.component.html',
  styleUrl: './find-people.component.scss'
})
export class FindPeopleComponent {
  constructor(public dialog: MatDialog, public dialogRef: MatDialogRef<FindPeopleComponent>){}

  onClose(): void {
    this.dialogRef.close();
  }
}
