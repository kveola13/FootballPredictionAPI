import { ComponentFixture, TestBed ***REMOVED*** from '@angular/core/testing';

import { FooterComponent ***REMOVED*** from './footer.component';

describe('FooterComponent', () => {
  let component: FooterComponent;
  let fixture: ComponentFixture<FooterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FooterComponent ]
    ***REMOVED***)
    .compileComponents();

    fixture = TestBed.createComponent(FooterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  ***REMOVED***);

  it('should create', () => {
    expect(component).toBeTruthy();
  ***REMOVED***);
***REMOVED***);
