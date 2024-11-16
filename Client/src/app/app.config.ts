import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { CredentialsInterceptor } from './utils/credentials-interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),  
    provideHttpClient(withInterceptors([CredentialsInterceptor])),
    provideRouter(routes), 
    provideAnimationsAsync(),  
    provideHttpClient() 
  ]
};
