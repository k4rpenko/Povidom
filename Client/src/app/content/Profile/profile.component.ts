import { Component } from '@angular/core';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent {
  user = {
    avatar: 'path-to-default-avatar.jpg',
    nickname: '',
    firstName: '',
    lastName: '',
    bio: ''
  };

  onAvatarChange(event: Event): void {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = () => {
        this.user.avatar = reader.result as string;
      };
      reader.readAsDataURL(file);
    }
  }

  onSubmit(): void {
    // Call your server API to save changes, e.g., /api/Admin/ChangUser
    console.log('Profile data:', this.user);
  }
}
