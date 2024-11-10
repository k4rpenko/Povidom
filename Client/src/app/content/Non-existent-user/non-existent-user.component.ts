import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-non-existent-user',
  standalone: true,
  imports: [],
  templateUrl: './non-existent-user.component.html',
  styleUrls: ['./non-existent-user.component.scss'] 
})
export class NonExistentUserComponent {
  constructor(private router: Router) {}

  goHome() {
    this.router.navigate(['/']); 
  }
}
