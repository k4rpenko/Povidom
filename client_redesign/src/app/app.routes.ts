import { Routes } from '@angular/router';
import { RegisterComponent } from './pages/authentication/register/register.component';
import { LoginComponent } from './pages/authentication/login/login.component';
import { HomeComponent } from './pages/home/home.component';
import { MessagesComponent } from './pages/messages/messages.component';
import { UserComponent } from './pages/user/user.component';
import {PremiumComponent} from './pages/premium/premium.component';

export const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'register', component: RegisterComponent },
  { path: 'login', component: LoginComponent },
  { path: 'home', component: HomeComponent },
  { path: 'message', component: MessagesComponent },
  { path: 'premium', component: PremiumComponent },
  { path: ':username', component: UserComponent },
];
