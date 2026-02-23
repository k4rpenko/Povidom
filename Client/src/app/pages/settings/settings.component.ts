import { Component } from '@angular/core';
import {BorderMainComponent} from "../../components/border-main/border-main.component";
import {HEADERComponent} from "../../components/header/header.component";
import {CommonModule} from '@angular/common';
import {IndexDBComponent} from "../../api/UserDB/index-db/index-db.component";
import { Profile } from "../../components/profile/profile";
import { UserProfil } from '../../data/interface/Users/UserProfil.interface';
import { UserCacheService } from '../../data/cache/user.service';
import { take } from 'rxjs';

@Component({
  selector: 'app-settings',
    imports: [CommonModule, HEADERComponent, IndexDBComponent, Profile],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.scss'
})
export class SettingsComponent {
  user?: UserProfil;

  Types: string = "";
  url: string = '';
  biography: string = '';
  author: string = '';
  username: string = '';
  IsLoading = true;

  constructor(
    private userCache: UserCacheService
  ){}

  onInput(event: Event, field: string) {
    const editableElement = event.target as HTMLElement;

    switch (field) {
      case 'url':
        this.url = editableElement.innerText;
        break;
      case 'biography':
        this.biography = editableElement.innerText;
        break;
      case 'author':
        this.author = editableElement.innerText;
        break;
      case 'username':
        this.username = editableElement.innerText;
        break;
    }
  }


  setType(value: string) {
    this.check(value);
    this.Types = value;
  }

  check(value: string){
    if(value === "Profile"){
      this.userCache.getUser()
        .pipe(take(1))
        .subscribe(user => {
          if (!user) return;
          this.user = { ...user };
          this.IsLoading = false;
      });
    }
  }
}
