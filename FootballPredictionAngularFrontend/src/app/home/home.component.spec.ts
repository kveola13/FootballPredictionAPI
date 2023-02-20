import { ComponentFixture, TestBed ***REMOVED*** from '@angular/core/testing';

import { HomeComponent ***REMOVED*** from './home.component';

describe('HomeComponent', () => {
  let component: HomeComponent;
  let fixture: ComponentFixture<HomeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ HomeComponent ]
    ***REMOVED***)
    .compileComponents();

    fixture = TestBed.createComponent(HomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  ***REMOVED***);

  it('should create', () => {
    expect(component).toBeTruthy();
  ***REMOVED***);
***REMOVED***);
