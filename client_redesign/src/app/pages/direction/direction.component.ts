import { Component } from '@angular/core';
import {Router} from '@angular/router';
import {CookieService} from 'ngx-cookie-service';

@Component({
  selector: 'app-direction',
  templateUrl: './direction.component.html',
  styleUrls: ['./direction.component.scss']
})
export class DirectionComponent {
  constructor(private router: Router, private cookieService: CookieService) {

    if (this.cookieService.check('authToken')) {
      console.log("go to home");
      this.router.navigate(['home']);
    } else {
      this.router.navigate(['home']);
    }

  }
}
