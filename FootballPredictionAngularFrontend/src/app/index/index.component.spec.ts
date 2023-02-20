import { ComponentFixture, TestBed ***REMOVED*** from '@angular/core/testing';

import { IndexComponent ***REMOVED*** from './index.component';

describe('IndexComponent', () => {
  let component: IndexComponent;
  let fixture: ComponentFixture<IndexComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ IndexComponent ]
    ***REMOVED***)
    .compileComponents();

    fixture = TestBed.createComponent(IndexComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  ***REMOVED***);

  it('should create', () => {
    expect(component).toBeTruthy();
  ***REMOVED***);
***REMOVED***);
