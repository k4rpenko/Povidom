import { Routes } from '@angular/router';
import { PageNotFoundComponent } from './pages/page-not-found/test.component';
import { PostsComponent } from './pages/posts/posts.component';
import { LoginComponent } from './pages/login/login.component';
import { MessageComponent } from './pages/message/message.component';

export const routes: Routes = [
    { path: '', component: LoginComponent},
    { path: 'home', component: PostsComponent },
    { path: 'message', component: MessageComponent },
    { path: '**', component: PageNotFoundComponent }
 ];
