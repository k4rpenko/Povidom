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

  GetFolowers(user_name: string) {
    return this.http.get<{users: UserProfil[]}>(`api/Fleets/Subscribers?user_name=${user_name}&size=0`, {
      headers: { 'Content-Type': 'application/json' },
      withCredentials: true
    });
  }

  GetSubscribers(user_name: string) {
    return this.http.get<{users: UserProfil[]}>(`/api/Fleets/Followers?user_name=${user_name}&size=0`, {
      headers: { 'Content-Type': 'application/json' },
      withCredentials: true
    });
  }

  GetUserDataNick(nick: string) {
    return this.http.get<{user: UserProfil}>(`api/Fleets/Profile?user_name=${nick}`, {
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

  PutSubscribers(user: string) {
    return this.http.put<{post: Post[]}>(`api/Fleets/Subscribers?user_name=${user}`, {
    headers: { 'Content-Type': 'application/json' },
    withCredentials: true
    });
  }

  DeleteSubscribers(user: string) {
    return this.http.delete<{post: Post[]}>(`api/Fleets/Subscribers?user_name=${user}`, {
    headers: { 'Content-Type': 'application/json' },
    withCredentials: true
    });
  }
}
