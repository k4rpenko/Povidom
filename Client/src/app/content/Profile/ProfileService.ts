import { ProfileService } from "./profile.service";

constructor(private profileService: ProfileService) {}

onSubmit(): void {
  this.profileService.updateUserProfile(this.user).subscribe(
    (response) => {
      console.log('Profile updated successfully', response);
    },
    (error) => {
      console.error('Error updating profile', error);
    }
  );
}
