import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { PostsComponent } from './pages/posts/posts.component';
import { MessageComponent } from './pages/message/message.component';
import { ProfileComponent } from './content/profile/profile.component';

export const routes: Routes = [
    { path: '', component: LoginComponent },
    { path: 'home', component: PostsComponent },
    { path: 'message', component: MessageComponent },
    { path: ':username', component: ProfileComponent }, 
];
