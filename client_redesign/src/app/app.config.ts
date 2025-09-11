import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { routes } from './app.routes';
import { HttpInterceptorFn } from '@angular/common/http';

const credentialsInterceptor: HttpInterceptorFn = (req, next) => {
  // Автоматично додаємо withCredentials до всіх запитів до API
  if (req.url.startsWith('/api/')) {
    req = req.clone({
      withCredentials: true
    });
  }
  return next(req);
};

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(
      withFetch(),
      withInterceptors([credentialsInterceptor])
    )
  ]
};
