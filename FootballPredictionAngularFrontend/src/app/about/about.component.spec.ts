import { ComponentFixture, TestBed ***REMOVED*** from '@angular/core/testing';

import { AboutComponent ***REMOVED*** from './about.component';

describe('AboutComponent', () => {
  let component: AboutComponent;
  let fixture: ComponentFixture<AboutComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AboutComponent ]
    ***REMOVED***)
    .compileComponents();

    fixture = TestBed.createComponent(AboutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  ***REMOVED***);

  it('should create', () => {
    expect(component).toBeTruthy();
  ***REMOVED***);
***REMOVED***);
