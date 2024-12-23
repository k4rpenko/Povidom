import { Component, inject, OnInit, Type } from "@angular/core";
import { ActivatedRoute, Router, RouterOutlet } from '@angular/router';
import { SideMenuComponent } from "../Global/Header/side-menu/side-menu.component";
import {CheckUser} from "./data/Global"
import { updateAccetsToken } from "./data/HTTP/POST/updateAccetsToken.service";
import { CookieService } from 'ngx-cookie-service';
import { CommonModule } from "@angular/common";
import { MemoryCacheService } from "./data/Cache/MemoryCacheService";


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, SideMenuComponent, CommonModule],
  providers: [CookieService],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit{
  title = 'Client';

  private lastRequestTime: Date | null = null;
  profileService = inject(updateAccetsToken);
  private readonly REQUEST_INTERVAL = 1500000;
  token!: string;
  showMainContent: boolean = true;

  constructor(private route: ActivatedRoute, private router: Router, private cookieService: CookieService, private cache: MemoryCacheService){
    if(this.cookieService.check('authToken')){
      const now = new Date();
      this.UpdateJWT(now);
    }
    else{
      this.router.navigate(['']);
    }
  }

  ngOnInit(): void {
    const now = new Date();
    this.UpdateJWT(now);
    this.router.events.subscribe(() => {
      this.showMainContent = this.router.url !== '/';
    });
  }

  UpdateJWT(now: Date) {
    if(this.cookieService.check('authToken')){
      if (!this.lastRequestTime || now.getTime() - this.lastRequestTime.getTime() > this.REQUEST_INTERVAL) {
        this.token = this.cookieService.get('authToken');
        this.lastRequestTime = now;
        if (this.token !== '' && this.token !== null) {
            this.profileService.updateAccetsToken(this.token).subscribe({
                next: (response) => {
                    const token = response.token;
                    CheckUser.Valid = true
                    this.cookieService.set('authToken', token);
                },
                error: (error) => {
                  const cookies = document.cookie.split(";");
                  for (let cookie of cookies) {
                      const cookieName = cookie.split("=")[0].trim();
                      document.cookie = `${cookieName}=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/;`;
                      this.cache.clearItem("User")
                  }
                }
            });
        }
      }
    }
    else{
      //this.router.navigate([`${window.location.origin}/login`]);
    }
  }
}
