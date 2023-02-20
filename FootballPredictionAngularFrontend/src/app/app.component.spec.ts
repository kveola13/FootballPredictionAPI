import { TestBed ***REMOVED*** from '@angular/core/testing';
import { RouterTestingModule ***REMOVED*** from '@angular/router/testing';
import { AppComponent ***REMOVED*** from './app.component';

describe('AppComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        RouterTestingModule
      ],
      declarations: [
        AppComponent
      ],
    ***REMOVED***).compileComponents();
  ***REMOVED***);

  it('should create the app', () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  ***REMOVED***);

  it(`should have as title 'FootballPredictionAngularFrontend'`, () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    expect(app.title).toEqual('FootballPredictionAngularFrontend');
  ***REMOVED***);

  it('should render title', () => {
    const fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement;
    expect(compiled.querySelector('.content span').textContent).toContain('FootballPredictionAngularFrontend app is running!');
  ***REMOVED***);
***REMOVED***);
