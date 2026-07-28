import { Component, ElementRef, HostListener, inject, Input, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { debounceTime, distinctUntilChanged, filter, interval, Observable, of, Subject, Subscription, switchMap } from 'rxjs';
import { RouterModule } from '@angular/router';
import { AddPostComponent } from '../../pages/add-post/add-post.component';
import { UserREST } from '../../../api/REST/user/UserData.service';
import { UserProfil } from '../../../data/interface/Users/UserProfil.interface';
import { UserCacheService } from '../../../data/cache/user.service';
import { AuthGuard } from '../../../guards/auth.guard';
import { SignalRService } from '../../../api/WebSocket/SignalR.service';
import { FindUser } from "../../pages/find-user/find-user";


@Component({
  selector: 'app-header',
  imports: [CommonModule, AddPostComponent, RouterModule, FindUser],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HEADERComponent implements OnInit, OnDestroy {
  @Input() url: string = '';
  find_user: boolean = false;
  users: UserProfil[] = [];
  Rest = inject(UserREST);

  private timerSub!: Subscription;
  private searchSubject = new Subject<string>();
  private subscription!: Subscription;
  user$!: Observable<UserProfil | null>;
  TOKEN!: Observable<boolean>;

  constructor(
    private userCache: UserCacheService,
    private guards: AuthGuard,
    private signalRService: SignalRService,

  ) {}

  ngOnInit() {
    this.TOKEN = this.guards.token.asObservable();

    this.userCache.loadUser().subscribe();
    this.user$ = this.userCache.user$;

    this.subscription = this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      filter(searchText => !!searchText && searchText.trim().length > 0 && /^[a-zA-Z1-9]+$/.test(searchText.trim())),
      switchMap(searchText => this.Rest.GetUsersDataNick(searchText))
    ).subscribe({
      next: (value) => {
        this.users = value.user;
        this.find_user = value.user.length > 0;
      },
      error: (err) => {
        this.users = [];
        this.find_user = false;
        console.log(err);
      }
    });
    
    this.signalRService.startConnection();
  }



  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  public onSearchInput(name: string){
    this.searchSubject.next(name);
  }

  isAddPostOpen = false;

  openAddPost() {
    this.isAddPostOpen = true;
  }

  closeAddPost() {
    this.isAddPostOpen = false;
  }

}
