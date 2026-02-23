import { Component, Input } from '@angular/core';
import { UserProfil } from '../../data/interface/Users/UserProfil.interface';
import { RouterModule } from '@angular/router';
import { UserREST } from '../../api/REST/user/UserData.service';
import { UserCacheService } from '../../data/cache/user.service';
import { UsersKnow } from "../user/users-know/users-know";
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-profile',
  imports: [RouterModule, UsersKnow, CommonModule, FormsModule],
  templateUrl: './profile.html',
  styleUrl: './profile.scss',
})
export class Profile {
  @Input({ required: true }) user!: UserProfil;
  @Input({ required: true }) type!: string;
  @Input({ required: true }) IsLoading!: boolean;

  editNameValue: string = "";
  editUsernameValue: string = "";

  isUsersArray = false;
  IsYou = false;
  NotFoundUser = false;
  loadingUser = true;
  loadingPosts = true;
  arrayType = 0;

  constructor(
    private userRest: UserREST,
    private userCache: UserCacheService
  ){}

  ngOnInit() {
    this.IsYou = this.userCache.checkUserName(this.user.userName!.toString());
    this.editNameValue = this.user.firstName!;
    this.editUsernameValue = this.user.userName!;
  }

  private optimisticFollow(state: boolean) {
    this.user.youFollower = state;
    this.user.followersAmount! += state ? 1 : -1;
  }

  subscribe(username: string) {
    this.optimisticFollow(true);

    this.userRest.PutSubscribers(username).subscribe({
      next: () => this.userCache.AddSubscriber(1),
      error: () => this.optimisticFollow(false)
    });
  }

  unsubscribe(username: string) {
    this.optimisticFollow(false);

    this.userRest.DeleteSubscribers(username).subscribe({
      next: () => this.userCache.AddSubscriber(-1),
      error: () => this.optimisticFollow(true)
    });
  }

  openUsersArray(type: number) {
    this.arrayType = type;
    this.isUsersArray = true;
  }

  closeUsersArray() {
    this.isUsersArray = false;
  }
  

  EdingAvatar(){

  }


  onUsernameChange(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.value.length > 10) {
      input.value = input.value.slice(0, 10);
      this.editUsernameValue = input.value;
    }
  }


  onNameChange(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.value.length > 10) {
      input.value = input.value.slice(0, 10);
      this.editNameValue = input.value;
    }
  }
}
