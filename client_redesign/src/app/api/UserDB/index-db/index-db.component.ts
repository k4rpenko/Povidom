import {Component, inject} from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { Router } from '@angular/router';
import {CheckCoockieService} from '../../PUT/verification/checkCoockie/CheckCoockie.service';

@Component({
  selector: 'app-index-db',
  imports: [],
  templateUrl: './index-db.component.html',
  styleUrl: './index-db.component.scss'
})
export class IndexDBComponent {
  Rest = inject(CheckCoockieService);

  constructor(private router: Router, private cookieService: CookieService) {

  }
}
