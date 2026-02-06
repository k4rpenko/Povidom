import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { UserProfil } from '../../../data/interface/Users/UserProfil.interface';
import { CommonModule } from '@angular/common';
import { UserREST } from '../../../api/REST/user/UserData.service';
import { Router, RouterModule } from '@angular/router';
import { UserCacheService } from '../../../data/cache/user.service';

@Component({
  selector: 'app-users-know',
  imports: [CommonModule, RouterModule],
  templateUrl: './users-know.html',
  styleUrl: './users-know.scss',
})
export class UsersKnow {
  @Output() closed = new EventEmitter<void>();
  @Input() user!: UserProfil;
  @Input() type!: number;
  @Input() YouUserName!: string;
  private router = inject(Router);
  users: UserProfil[] = [];
  loading = true;

  UserRest = inject(UserREST);

  constructor(
    private userCache: UserCacheService,
  ) {}
    
  ngOnInit() {
    const request = this.type === 1
        ? this.UserRest.GetFolowers(this.user.userName!)
        : this.type === 2
        ? this.UserRest.GetSubscribers(this.user.userName!)
        : null;
    if (!request) {
      this.close();
      return;
    }

    request.subscribe({
      next: (res) => {
        this.users = res.users;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.close();
      },
    });
  }

  Subscribers(user: UserProfil){
    user.youFollower = !user.youFollower
    user.followersAmount!++;
    this.UserRest.PutSubscribers(user.userName!).subscribe({
      next: () => {
        this.userCache.AddSubscriber(1);
      },
      error: (error) => {
        user.youFollower = !user.youFollower;
        user.followersAmount!--;
      }
    })
  }

  UnSubscribers(user: UserProfil){
    user.youFollower = !user.youFollower
    user.followersAmount!--;
    this.UserRest.DeleteSubscribers(user.userName!).subscribe({
      next: () => {
        this.userCache.AddSubscriber(-1);
      },
      error: (error) => {
        user.youFollower = !user.youFollower;
        user.followersAmount!++;
      }
    })
  }

  openAnotherProfile(user: UserProfil){
    this.close();
    this.router.navigate(['/', user.userName]);
  }

  close() {
    this.closed.emit();
  }
}
