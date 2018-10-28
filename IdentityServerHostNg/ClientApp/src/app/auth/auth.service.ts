import { Injectable, NgZone } from '@angular/core';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material';
import { OAuthErrorEvent, OAuthService } from 'angular-oauth2-oidc';
import { BehaviorSubject, combineLatest, Observable, ReplaySubject } from 'rxjs';
import { filter, map } from 'rxjs/operators';

// import { authConfig } from './auth-config';

@Injectable({ providedIn: 'root' })
export class AuthService {

  private isAuthenticatedSubject$ = new BehaviorSubject<boolean>(false);
  public isAuthenticated$ = this.isAuthenticatedSubject$.asObservable();

  private isDoneLoadingSubject$ = new ReplaySubject<boolean>();
  public isDoneLoading$ = this.isDoneLoadingSubject$.asObservable();

  private isAdministrationApiSubject$ = new ReplaySubject<boolean>();
  public isAdministrationApi$ = this.isAdministrationApiSubject$.asObservable();

  private isAdvancedApiSubject$ = new ReplaySubject<boolean>();
  public isAdvancedApi$ = this.isAdvancedApiSubject$.asObservable();

  private isBasicApiSubject$ = new ReplaySubject<boolean>();
  public isBasicApi$ = this.isBasicApiSubject$.asObservable();

  /**
   * Publishes `true` if and only if (a) all the asynchronous initial
   * login calls have completed or errorred, and (b) the user ended up
   * being authenticated.
   *
   * In essence, it combines:
   *
   * - the latest known state of whether the user is authorized
   * - whether the ajax calls for initial log in have all been done
   */
  public canActivateProtectedRoutes$: Observable<boolean> = combineLatest(this.isAuthenticated$, this.isDoneLoading$)
    .pipe(map(values => values.every(b => b)));

  constructor(private oauthService: OAuthService, private router: Router, private snackBar: MatSnackBar, private zone: NgZone) {
    window.addEventListener('storage', (event) => {
      // This is tricky, as it might cause race conditions where access_token is set in another tab before everything is said and done there.
      // The `key` is `null` if the event was caused by `.clear()`
      if (event.key !== 'access_token' && event.key !== null) return;
      this.notifyWarning('Noticed changes to access_token (most likely from another tab), updating isAuthenticated');
      this.isAuthenticatedSubject$.next(this.oauthService.hasValidAccessToken());
      if (!this.oauthService.hasValidAccessToken()) this.navigateToLoginPage();
    });

    this.oauthService.events.subscribe(_ => this.isAuthenticatedSubject$.next(this.oauthService.hasValidAccessToken()));
    this.oauthService.events.subscribe(_ => this.isAdministrationApiSubject$.next(this.isApiLevel('administration_api')));
    this.oauthService.events.subscribe(_ => this.isAdvancedApiSubject$.next(this.isApiLevel('advanced_api')));
    this.oauthService.events.subscribe(_ => this.isBasicApiSubject$.next(this.isApiLevel('basic_api')));
    this.oauthService.events.subscribe(event => this.notifyEvent(event)); // useful for debugging

    this.oauthService.events
      .pipe(filter(e => ['token_received'].includes(e.type)))
      .subscribe(e => this.oauthService.loadUserProfile());

    this.oauthService.events
      .pipe(filter(e => ['session_terminated', 'session_error'].includes(e.type)))
      .subscribe(e => this.navigateToLoginPage());

    // alternatively can be configured in app.module
    // this.oauthService.configure(authConfig);
    // this.oauthService.tokenValidationHandler = new JwksValidationHandler();
    // this.oauthService.setupAutomaticSilentRefresh();
    this.oauthService.requireHttps = false;
    this.oauthService.loadDiscoveryDocumentAndTryLogin();
  }

  public runInitialLoginSequence(): Promise<void> {
    // 0. LOAD CONFIG
    // First we have to check to see how the IdServer is currently configured
    this.isDoneLoadingSubject$.next(false);
    return this.oauthService.loadDiscoveryDocument()

      // For demo purposes, we pretend the previous call was very slow
      // .then(() => new Promise(resolve => setTimeout(() => resolve(), 100)))

      // 1. HASH LOGIN
      // Try to log in via hash fragment after redirect back from IdServer from initImplicitFlow
      .then(() => this.oauthService.tryLogin({
        onTokenReceived: (info) => { console.debug('state', info.state); }
      }))
      .then(_ => {
        if (this.oauthService.hasValidAccessToken()) {
          return Promise.resolve();
        }

        // 2. SILENT LOGIN
        // Try to log in via silent refresh because the IdServer might have a cookie to remember the user,
        // so we can prevent doing a redirect
        return this.oauthService.silentRefresh()
          .then(_ => Promise.resolve())
          .catch(result => {
            // Subset of situations from https://openid.net/specs/openid-connect-core-1_0.html#AuthError
            // Only the ones where it's reasonably sure that sending the user to the IdServer will help
            const errorResponsesRequiringUserInteraction = [
              'interaction_required',
              'login_required',
              'account_selection_required',
              'consent_required',
            ];

            if (result && result.reason && errorResponsesRequiringUserInteraction.indexOf(result.reason.error) >= 0) {

              // 3. ASK FOR LOGIN
              // At this point we know for sure that we have to ask the user to log in,
              // so we redirect them to the IdServer to enter credentials
              //
              // Enable this to ALWAYS force a user to login.
              this.oauthService.initImplicitFlow();
              //
              // Instead, we'll now do this:
              // this.notifyWarning('User interaction is needed to log in, we will wait for the user to manually log in.');
              return Promise.resolve();
            }

            // We can't handle the truth, just pass on the problem to the next handler
            return Promise.reject(result);
          });
      })
      .then(() => {
        this.isDoneLoadingSubject$.next(true);

        // Check for the strings 'undefined' and 'null' just to be sure.
        // Our current login(...) should never have this, but in case someone ever calls
        // initImplicitFlow(undefined | null) this could happen.
        if (this.oauthService.state && this.oauthService.state !== 'undefined' && this.oauthService.state !== 'null') {
          this.notify('There was state, so we are sending you to: ' + this.oauthService.state);
          this.router.navigateByUrl(this.oauthService.state);
        }
      })
      .catch(() => this.isDoneLoadingSubject$.next(true));
  }

  private notifyEvent(event): void {
    if (event instanceof OAuthErrorEvent) {
      console.error(event);
      this.showSnackBar('ERROR: ' + event.type);
    } else {
      console.log(event);
      this.showSnackBar(event.type);
    }
  }
  public notifyError(message: string): void {
    console.error(message);
    this.showSnackBar('ERROR: ' + message);
  }
  private notifyWarning(message: string): void {
    console.warn(message);
    this.showSnackBar('WARN: ' + message);
  }
  private notify(message: string): void {
    console.log(message);
    this.showSnackBar(message);
  }
  private showSnackBar(message: string): void {
    this.zone.run(() => { this.snackBar.open(message, "OK", { duration: 5000 }); });
  }

  public login(targetUrl?: string) { this.oauthService.initImplicitFlow(encodeURIComponent(targetUrl || this.router.url)); }
  public logout() { this.oauthService.logOut(); }
  public refresh() { this.oauthService.silentRefresh(); }
  public hasValidAccessToken() { return this.oauthService.hasValidAccessToken(); }

  private isApiLevel(name: string): boolean {
    const claims = this.oauthService.getIdentityClaims();
    const api = claims ? claims['api_level'] : '-';
    if (api instanceof Array)
      return api.includes(name);
    return api === name;
  }

  private navigateToLoginPage() { this.oauthService.initImplicitFlow(); }

  // These normally won't be exposed from a service like this, but for debugging it makes sense.
  public get accessToken() { return this.oauthService.getAccessToken(); }
  public get identityClaims() { return this.oauthService.getIdentityClaims(); }
  public get grantedScopes() { return this.oauthService.getGrantedScopes(); }
  public get idToken() { return this.oauthService.getIdToken(); }
  public get logoutUrl() { return this.oauthService.logoutUrl; }
  public get accessTokenExpiration() { return new Date(this.oauthService.getAccessTokenExpiration()); }
  public get idTokenExpiration() { return new Date(this.oauthService.getIdTokenExpiration()); }
  public get resource() { return this.oauthService.resource; }
  public get scope() { return this.oauthService.scope; }
  public get state() { return this.oauthService.state; }
  public get options() { return this.oauthService.options; }
}
