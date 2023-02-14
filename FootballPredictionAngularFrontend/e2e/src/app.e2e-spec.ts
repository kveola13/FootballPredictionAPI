import { browser, logging ***REMOVED*** from 'protractor';
import { AppPage ***REMOVED*** from './app.po';

describe('workspace-project App', () => {
  let page: AppPage;

  beforeEach(() => {
    page = new AppPage();
  ***REMOVED***);

  it('should display welcome message', async () => {
    await page.navigateTo();
    expect(await page.getTitleText()).toEqual('FootballPredictionAngularFrontend app is running!');
  ***REMOVED***);

  afterEach(async () => {
    // Assert that there are no errors emitted from the browser
    const logs = await browser.manage().logs().get(logging.Type.BROWSER);
    expect(logs).not.toContain(jasmine.objectContaining({
      level: logging.Level.SEVERE,
    ***REMOVED*** as logging.Entry));
  ***REMOVED***);
***REMOVED***);
