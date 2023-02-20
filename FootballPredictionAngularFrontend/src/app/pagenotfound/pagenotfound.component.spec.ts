import { ComponentFixture, TestBed ***REMOVED*** from '@angular/core/testing';

import { PagenotfoundComponent ***REMOVED*** from './pagenotfound.component';

describe('PagenotfoundComponent', () => {
  let component: PagenotfoundComponent;
  let fixture: ComponentFixture<PagenotfoundComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PagenotfoundComponent ]
    ***REMOVED***)
    .compileComponents();

    fixture = TestBed.createComponent(PagenotfoundComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  ***REMOVED***);

  it('should create', () => {
    expect(component).toBeTruthy();
  ***REMOVED***);
***REMOVED***);
