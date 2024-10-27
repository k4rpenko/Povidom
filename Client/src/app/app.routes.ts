import { Routes } from '@angular/router';
import { PageNotFoundComponent } from './content/page-not-found/test.component';
import { SideMenuComponent } from './content/side-menu/side-menu.component';

export const routes: Routes = [
    { path: '', component: SideMenuComponent },

    { path: '**', component: PageNotFoundComponent }
 ];
