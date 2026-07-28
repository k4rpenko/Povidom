import { Component, inject, OnInit, OnDestroy, HostListener, ElementRef, Input, output, Output, EventEmitter } from '@angular/core';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, filter, switchMap } from 'rxjs/operators';
import { UserREST } from '../../../api/REST/user/UserData.service';
import { UserProfil } from '../../../data/interface/Users/UserProfil.interface';
import { FindUser } from "../../pages/find-user/find-user";
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-users',
  templateUrl: './users.html',
  styleUrls: ['./users.scss'],
  imports: [FindUser, CommonModule],
})
export class Users implements OnInit, OnDestroy{
  user!: UserProfil;
  find_user: boolean = false;
  users: UserProfil[] = [];
  Rest = inject(UserREST);
  @Output() userSelected = new EventEmitter<UserProfil>();

  public onUserSelected(user: UserProfil) {
    this.userSelected.emit(user);
  }


  private searchSubject = new Subject<string>();
  private subscription!: Subscription;

  ngOnInit() {
    this.subscription = this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      filter(searchText => !!searchText && searchText.trim().length > 0 && /^[a-zA-Z1-9]+$/.test(searchText.trim())),
      switchMap(searchText => this.Rest.GetUsersDataNick(searchText))
    ).subscribe({
      next: (value) => {
        this.users = value.user;
        this.find_user = value.user.length > 0;
        console.log(this.find_user, this.users);
      },
      error: (err) => {
        this.users = [];
        this.find_user = false;
        console.log(err);
      }
    });
    
  }
  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  public onSearchInput(name: string){
    this.searchSubject.next(name);
  }
}