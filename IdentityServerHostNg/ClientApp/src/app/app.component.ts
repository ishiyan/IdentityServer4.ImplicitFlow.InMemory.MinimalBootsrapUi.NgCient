import { Component } from '@angular/core';
import * as jwt_decode from "jwt-decode";
import { Observable } from 'rxjs';

import { AuthService } from './auth/auth.service';

@Component({
  selector: 'app-root',
  styles: [
    '.button { margin-right: 8px; margin-bottom: 8px; }',
    '.fill-remaining-space {flex: 1 1 auto;}',
    '.card { width: 90%; }',
    '.textarea { width: 100%; }'
  ],
  template: `<div class="container-fluid">
      <mat-toolbar color="primary" class="mat-elevation-z8">
        <mat-toolbar-row>
          <a mat-button routerLinkActive="active" routerLink="/publicapi" *ngIf='(isAuthenticated | async) && (isDoneLoading | async)'>Public</a>
          <a mat-button routerLinkActive="active" routerLink="/adminapi" *ngIf='isAdministrationApi | async'>Admin</a>
          <a mat-button routerLinkActive="active" routerLink="/advancedapi" *ngIf='isAdvancedApi | async'>Advanced</a>
          <a mat-button routerLinkActive="active" routerLink="/basicapi" *ngIf='isBasicApi | async'>Basic</a>
          <span class='fill-remaining-space'></span>
          <button mat-button (click)='logout()' *ngIf='isAuthenticated | async'>{{name}} logout</button>
        </mat-toolbar-row>
      </mat-toolbar>
      <p></p>
      <mat-card class='card' style='background: #69F0AE;'>
        <mat-card-header>
          <mat-card-title>Router outlet</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <router-outlet></router-outlet>
        </mat-card-content>
      </mat-card>
      <p></p>
      <mat-card class='card'>
        <mat-card-title *ngIf='!(isDoneLoading | async)'>Authenticating...</mat-card-title>
        <mat-card-content>
          <mat-progress-bar mode="indeterminate" *ngIf='!(isDoneLoading | async)'></mat-progress-bar>
          <p>
            <button mat-raised-button color="primary" (click)='login()' class='button' *ngIf='!(isAuthenticated | async)'>login</button>
            <button mat-raised-button color="primary" (click)='logout()' class='button' *ngIf='(isAuthenticated | async)'>logout</button>
            <button mat-raised-button color="primary" (click)='logoutExternally()' class='button' *ngIf='(isAuthenticated | async)'>logout externally...</button>
            <button mat-raised-button color="primary" (click)='refresh()' class='button' *ngIf='(isAuthenticated | async)'>force silent refresh</button> 
            <button mat-raised-button color="primary" (click)='reload()' class='button'>reload page</button> 
            <button mat-raised-button color="primary" (click)='clearStorage()' class='button'>clear local storage</button>
          </p>
          <p>
            Explore <a href=".well-known/openid-configuration">well-known openid configuration</a>.
          </p>
          <mat-chip-list selectable='false'>
            <mat-chip>IsAuthenticated: {{isAuthenticated | async}}</mat-chip>
            <mat-chip>HasValidAccessToken: {{hasValidAccessToken}}</mat-chip>
            <mat-chip>IsDoneLoading: {{canActivateProtectedRoutes | async}}</mat-chip>
            <mat-chip>CanActivateProtectedRoutes: {{canActivateProtectedRoutes | async}}</mat-chip>
            <mat-chip>IsAdministrationApi: {{isAdministrationApi | async}}</mat-chip>
            <mat-chip>IsAdvancedApi: {{isAdvancedApi | async}}</mat-chip>
            <mat-chip>IsBasicApi: {{isBasicApi | async}}</mat-chip>
          </mat-chip-list>
        </mat-card-content>
      </mat-card>
      <p></p>
      <mat-card class='card'>
        <mat-card-header>
          <mat-card-title>Identity token</mat-card-title>
        </mat-card-header>
        <mat-card-subtitle>
          Expiration: {{idTokenExpiration}}
        </mat-card-subtitle>
        <mat-card-content>
          <textarea readonly cdkTextareaAutosize='true' cdkAutosizeMaxRows='100' class='textarea'>{{idToken}}</textarea>
          <p>Decoded</p>
          <textarea readonly cdkTextareaAutosize='true' cdkAutosizeMaxRows='100' class='textarea'>{{idTokenDecoded | json}}</textarea>
          <p>Identity claims</p>
          <textarea readonly cdkTextareaAutosize='true' cdkAutosizeMaxRows='100' class='textarea'>{{identityClaims | json}}</textarea>
        </mat-card-content>
      </mat-card>
      <p></p>
      <mat-card class='card'>
        <mat-card-header>
          <mat-card-title>Access token</mat-card-title>
        </mat-card-header>
        <mat-card-subtitle>
          Expiration: {{accessTokenExpiration}}
        </mat-card-subtitle>
        <mat-card-content>
          <textarea readonly cdkTextareaAutosize='true' cdkAutosizeMaxRows='100' class='textarea'>{{accessToken}}</textarea>
        </mat-card-content>
        <p>Decoded</p>
        <textarea readonly cdkTextareaAutosize='true' cdkAutosizeMaxRows='100' class='textarea'>{{accessTokenDecoded | json}}</textarea>
      </mat-card>
      <p></p>
      <mat-card class='card'>
        <mat-card-header>
          <mat-card-title>Granted scopes</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <textarea readonly cdkTextareaAutosize='true' cdkAutosizeMaxRows='100' class='textarea'>{{grantedScopes | json}}</textarea>
        </mat-card-content>
      </mat-card>
      <p></p>
      <mat-card class='card'>
        <mat-card-header>
          <mat-card-title>resource</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <textarea readonly cdkTextareaAutosize='true' cdkAutosizeMaxRows='100' class='textarea'>{{resource}}</textarea>
        </mat-card-content>
      </mat-card>
      <p></p>
      <mat-card class='card'>
        <mat-card-header>
          <mat-card-title>scope</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <textarea readonly cdkTextareaAutosize='true' cdkAutosizeMaxRows='100' class='textarea'>{{scope}}</textarea>
        </mat-card-content>
      </mat-card>
      <p></p>
      <mat-card class='card'>
        <mat-card-header>
          <mat-card-title>state</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <textarea readonly cdkTextareaAutosize='true' cdkAutosizeMaxRows='100' class='textarea'>{{state | json}}</textarea>
        </mat-card-content>
      </mat-card>
      <p></p>
      <mat-card class='card'>
        <mat-card-header>
          <mat-card-title>options</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <textarea readonly cdkTextareaAutosize='true' cdkAutosizeMaxRows='100' class='textarea'>{{options | json}}</textarea>
        </mat-card-content>
      </mat-card>
  </div>`,
})
export class AppComponent {
  isAuthenticated: Observable<boolean>;
  isDoneLoading: Observable<boolean>;
  canActivateProtectedRoutes: Observable<boolean>;

  constructor(private authService: AuthService) {
    this.isAuthenticated = this.authService.isAuthenticated$;
    this.isDoneLoading = this.authService.isDoneLoading$;
    this.canActivateProtectedRoutes = this.authService.canActivateProtectedRoutes$;
    this.authService.runInitialLoginSequence();
  }

  login() { this.authService.login(); }
  logout() { this.authService.logout(); }
  logoutExternally() { window.open(this.authService.logoutUrl); }
  refresh() { this.authService.refresh(); }
  reload() { window.location.reload(); }
  clearStorage() { localStorage.clear(); }

  get name() { return this.authService.identityClaims ? this.authService.identityClaims['name'] : '-'; }
  get isAdministrationApi() { return this.authService.isAdministrationApi$; }
  get isAdvancedApi() { return this.authService.isAdvancedApi$; }
  get isBasicApi() { return this.authService.isBasicApi$; }

  get hasValidAccessToken() { return this.authService.hasValidAccessToken(); }
  get accessToken() { return this.authService.accessToken; }
  get identityClaims() { return this.authService.identityClaims; }
  get grantedScopes() { return this.authService.grantedScopes; }
  get idToken() { return this.authService.idToken; }
  get accessTokenExpiration() { return this.authService.accessTokenExpiration; }
  get idTokenExpiration() { return this.authService.idTokenExpiration; }
  get resource() { return this.authService.resource; }
  get scope() { return this.authService.scope; }
  get state() { return this.authService.state; }
  get idTokenDecoded() {
    try {
      return jwt_decode(this.authService.idToken);
    } catch (e) {
      return '';
    }
  }
  get accessTokenDecoded() {
    try {
      return jwt_decode(this.authService.accessToken);
    } catch (e) {
      return '';
    }
  }
  get options() { return this.authService.options; }
}
