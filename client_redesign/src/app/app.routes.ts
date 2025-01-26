import { Routes } from '@angular/router';
import {DirectionComponent} from './pages/direction/direction.component';
import {RegisterComponent} from './pages/authentication/register/register.component';
import {LoginComponent} from './pages/authentication/login/login.component';
import {HomeComponent} from './pages/home/home.component';

export const routes: Routes = [
  { path: '', component: DirectionComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'login', component: LoginComponent },
  { path: 'home', component: HomeComponent },
];
