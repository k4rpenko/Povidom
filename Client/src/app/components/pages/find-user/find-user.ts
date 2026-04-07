import { Component, ElementRef, EventEmitter, HostListener, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserProfil } from '../../../data/interface/Users/UserProfil.interface';

@Component({
  selector: 'app-find-user',
  imports: [CommonModule],
  templateUrl: './find-user.html',
  styleUrl: './find-user.scss',
})
export class FindUser {
  @Input({ required: true }) type!: string;
  @Input({ required: true }) users!: UserProfil[];
  @Input({ required: true }) find_user!: boolean;
  @Output() userSelected = new EventEmitter<UserProfil>();
  
  constructor(
    private elementRef: ElementRef,
  ){}
  
  @HostListener('document:click', ['$event'])
  onClickOutside(event: MouseEvent){
    if (!this.elementRef.nativeElement.contains(event.target)) {
      this.find_user = false;
    }
  }

  public sendUser(user: UserProfil){
    this.userSelected.emit(user);
  }
}