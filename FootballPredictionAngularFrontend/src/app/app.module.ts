import { NgModule ***REMOVED*** from '@angular/core';
import { BrowserModule ***REMOVED*** from '@angular/platform-browser';

import { AppRoutingModule ***REMOVED*** from './app-routing.module';
import { AppComponent ***REMOVED*** from './app.component';
import { HeaderComponent ***REMOVED*** from './header/header.component';
import { FooterComponent ***REMOVED*** from './footer/footer.component';

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    FooterComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
***REMOVED***)
export class AppModule { ***REMOVED***
