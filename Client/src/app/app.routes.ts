import { Routes } from '@angular/router';
import { PostsComponent } from './pages/home/posts.component';
import { MessageComponent } from './pages/messages/message.component';
import { UsersComponent } from './pages/user/users.component';
import { SettingsComponent } from './pages/user/settings/settings.component';
import { CommentsPostComponent } from './pages/comments-post/comments-post.component';
import { Auth } from './pages/auth/auth.component';

export const routes: Routes = [
    { path: '', component: Auth},
    { path: 'home', component: PostsComponent },
    { path: 'message', component: MessageComponent },
    { path: 'comment/:post', component: CommentsPostComponent },
    { path: ':username', component: UsersComponent },
    { path: 'settings', component: SettingsComponent },
];
