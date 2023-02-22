import { enableProdMode ***REMOVED*** from '@angular/core';
import { platformBrowserDynamic ***REMOVED*** from '@angular/platform-browser-dynamic';

import { AppModule ***REMOVED*** from './app/app.module';
import { environment ***REMOVED*** from './environments/environment';

if (environment.production) {
  enableProdMode();
***REMOVED***

platformBrowserDynamic().bootstrapModule(AppModule)
  .catch(err => console.error(err));
