import { Routes } from '@angular/router';
import { PageNotFoundComponent } from './content/page-not-found/test.component';
import { PostsComponent } from './content/Main/posts/posts.component';

export const routes: Routes = [
    { path: '', component: PostsComponent },
    { path: '**', component: PageNotFoundComponent }
 ];
