import { Routes } from '@angular/router';
import { NonExistentUserComponent } from './content/Non-existent-user/non-existent-user.component';
import { PostsComponent } from './content/Main/posts/posts.component';

export const routes: Routes = [
    { path: '', component: PostsComponent },
    { path: 'Користувач/:username', component: NonExistentUserComponent },
    { path: '**', component: NonExistentUserComponent } 
];
