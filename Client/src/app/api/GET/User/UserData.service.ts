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
    // GetAnotherUserData() {
    //     return this.http.get<{post: Post[]}>(`api/SpacePosts/GetPosts`, {
    //     headers: { 'Content-Type': 'application/json' },
    //     withCredentials: true
    //     });
    // }
    // GetUserSearch() {
    //     return this.http.get<{post: Post[]}>(`api/SpacePosts/GetPosts`, {
    //     headers: { 'Content-Type': 'application/json' },
    //     withCredentials: true
    //     });
    // }
    // GetUserPremium() {
    //     return this.http.get<{post: Post[]}>(`api/SpacePosts/GetPosts`, {
    //     headers: { 'Content-Type': 'application/json' },
    //     withCredentials: true
    //     });
    // }
}
