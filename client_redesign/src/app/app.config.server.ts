import { mergeApplicationConfig, ApplicationConfig } from '@angular/core';
import { appConfig } from './app.config';

const serverConfig: ApplicationConfig = {
  providers: [
    // Вимкнули SSR для уникнення дублювання компонентів
  ]
};

export const config = mergeApplicationConfig(appConfig, serverConfig);
