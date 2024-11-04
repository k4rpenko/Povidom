import { Routes } from '@angular/router';
import { PageNotFoundComponent } from './pages/page-not-found/test.component';
import { PostsComponent } from './pages/posts/posts.component';
import { LoginComponent } from './pages/login/login.component';

export const routes: Routes = [
    { path: '', component: LoginComponent},
    { path: 'home', component: PostsComponent },
    { path: '**', component: PageNotFoundComponent }
 ];
