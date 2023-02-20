import { NgModule ***REMOVED*** from '@angular/core';
import { BrowserModule ***REMOVED*** from '@angular/platform-browser';

import { AppRoutingModule ***REMOVED*** from './app-routing.module';
import { AppComponent ***REMOVED*** from './app.component';
import { HeaderComponent ***REMOVED*** from './header/header.component';

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
***REMOVED***)
export class AppModule { ***REMOVED***
