import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Post } from '../../../data/interface/Post/Post.interface';
import { UserProfil } from '../../../data/interface/Users/UserProfil.interface';

@Injectable({
  providedIn: 'root'
})
export class UserREST {
  http = inject(HttpClient)
  constructor() { }

    GetUserID() {
        return this.http.get<{ID: string}>(`api/AccountSettings/ID`, {
        headers: { 'Content-Type': 'application/json' },
        withCredentials: true
        });
    }

    GetUserData() {
        return this.http.get<{user: UserProfil}>(`/api/Fleets`, {
        headers: { 'Content-Type': 'application/json' },
        withCredentials: true
        });
    }

    GetUserDataNick(nick: string) {
      return this.http.get<{user: UserProfil}>(`api/Fleets/Profile?Nick=${nick}`, {
        headers: { 'Content-Type': 'application/json' },
        withCredentials: true
      });
    }
    
    GetUserPosts(user: string) {
      return this.http.get<{post: Post[]}>(`api/SpacePosts/GetUserPost?user_name=${user}`, {
      headers: { 'Content-Type': 'application/json' },
      withCredentials: true
      });
    }
}
