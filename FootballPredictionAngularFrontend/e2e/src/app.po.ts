import { browser, by, element ***REMOVED*** from 'protractor';

export class AppPage {
  async navigateTo(): Promise<unknown> {
    return browser.get(browser.baseUrl);
  ***REMOVED***

  async getTitleText(): Promise<string> {
    return element(by.css('app-root .content span')).getText();
  ***REMOVED***
***REMOVED***
