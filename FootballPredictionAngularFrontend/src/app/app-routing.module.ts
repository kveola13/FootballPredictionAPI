import { NgModule ***REMOVED*** from '@angular/core';
import { RouterModule, Routes ***REMOVED*** from '@angular/router';
import { AboutComponent ***REMOVED*** from './about/about.component';
import { HomeComponent ***REMOVED*** from './home/home.component';
import { IndexComponent ***REMOVED*** from './index/index.component';
import { PagenotfoundComponent ***REMOVED*** from './pagenotfound/pagenotfound.component';

const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' ***REMOVED***,
  { path: 'home', component: HomeComponent, title: 'Home' ***REMOVED***,
  { path: 'index', component: IndexComponent, title: 'Index' ***REMOVED***,
  { path: 'about', component: AboutComponent, title: 'About' ***REMOVED***,
  { path: '**', component: PagenotfoundComponent ***REMOVED***,
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
***REMOVED***)
export class AppRoutingModule {***REMOVED***
