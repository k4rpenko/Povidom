import { Routes } from '@angular/router';
import { RegisterComponent } from './pages/authentication/register/register.component';
import { LoginComponent } from './pages/authentication/login/login.component';
import { HomeComponent } from './pages/home/home.component';
import { MessagesComponent } from './pages/messages/messages.component';
import { UserComponent } from './pages/user/user.component';
import {PremiumComponent} from './pages/premium/premium.component';
import {NotificationComponent} from './pages/notification/notification.component';
import {SettingsComponent} from './pages/settings/settings.component';
import { SavedComponent } from './pages/saved/saved.component';
import { AuthGuard } from './guards/auth.guard';
import { PostID } from './pages/user/post/post';
//import { AuthGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'register', component: RegisterComponent },
  { path: 'login', component: LoginComponent },
  
  // { path: 'home', component: HomeComponent },
  // { path: 'message', component: MessagesComponent },
  // { path: 'premium', component: PremiumComponent },
  // { path: 'notification', component: NotificationComponent },
  // { path: 'settings', component: SettingsComponent },
  // { path: 'saved', component: SavedComponent },

  { 
    path: 'home', 
    component: HomeComponent,
    canActivate: [AuthGuard]
  },
  { 
    path: 'message', 
    component: MessagesComponent,
    canActivate: [AuthGuard]
  },
  { 
    path: 'premium', 
    component: PremiumComponent,
    canActivate: [AuthGuard]
  },
  { 
    path: 'notification', 
    component: NotificationComponent,
    canActivate: [AuthGuard]
  },
  { 
    path: 'saved', 
    component: SavedComponent, 
    canActivate: [AuthGuard] 
  },
  { 
    path: 'settings', 
    component: SettingsComponent,
    canActivate: [AuthGuard]
  },
  { path: ':username', component: UserComponent },
  { path: 'post/:id', component: PostID },
];
