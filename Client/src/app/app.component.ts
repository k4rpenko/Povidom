import { Component, inject, OnInit, Type } from "@angular/core";
import { ActivatedRoute, Router, RouterOutlet } from '@angular/router';
import { SideMenuComponent } from "./content/Header/side-menu/side-menu.component";
import {CheckUser} from "./data/Сheck-User"
import { updateAccetsToken } from "./data/HTTP/POST/updateAccetsToken.service";
import { CookieService } from 'ngx-cookie-service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, SideMenuComponent],
  providers: [CookieService],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'] 
})
export class AppComponent {
  title = 'Client';
  private lastRequestTime: Date | null = null;
  profileService = inject(updateAccetsToken);
  private readonly REQUEST_INTERVAL = 1500000;
  token!: string;

  constructor(private route: ActivatedRoute, private router: Router, private cookieService: CookieService){
    if(this.cookieService.check('authToken')){
      const now = new Date();
      this.UpdateJWT(now);
    }
  }

  ngOnInit(): void {
    const now = new Date();
    this.UpdateJWT(now);
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
                    this.router.navigate(['/']);
                },
                error: (error) => {
                    const cookies = document.cookie.split(";");
                    for (let cookie of cookies) {
                        const cookieName = cookie.split("=")[0].trim(); 
                        document.cookie = `${cookieName}=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/;`;
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
