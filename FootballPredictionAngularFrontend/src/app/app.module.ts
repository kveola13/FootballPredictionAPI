import { NgModule ***REMOVED*** from '@angular/core';
import { BrowserModule ***REMOVED*** from '@angular/platform-browser';

import { AppRoutingModule ***REMOVED*** from './app-routing.module';
import { AppComponent ***REMOVED*** from './app.component';
import { HeaderComponent ***REMOVED*** from './header/header.component';
import { FooterComponent ***REMOVED*** from './footer/footer.component';
import { IndexComponent ***REMOVED*** from './index/index.component';
import { AboutComponent ***REMOVED*** from './about/about.component';
import { PagenotfoundComponent ***REMOVED*** from './pagenotfound/pagenotfound.component';

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    FooterComponent,
    IndexComponent,
    AboutComponent,
    PagenotfoundComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
***REMOVED***)
export class AppModule { ***REMOVED***
