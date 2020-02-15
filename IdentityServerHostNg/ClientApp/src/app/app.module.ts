
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';
import { OAuthModule, OAuthStorage, AuthConfig, JwksValidationHandler, OAuthModuleConfig, ValidationHandler } from 'angular-oauth2-oidc';

import { AppComponent } from './app.component';
import { MaterialModule } from './material.module';

import { authModuleConfig } from './auth/auth-module-config';
import { AuthService } from './auth/auth.service';
import { authConfig } from './auth/auth-config';
import { AuthGuard } from './auth/auth-guard.service';
import { AuthGuardAdministrationApi } from './auth/auth-guard-administration-api.service';
import { AuthGuardAdvancedApi } from './auth/auth-guard-advanced-api.service';
import { AuthGuardBasicApi } from './auth/auth-guard-basic-api.service';

import { ApiService } from './api/api.service';
import { ApiComponent } from './api/api.component';
import { PublicApiComponent } from './api/public-api.component';
import { AdministrationApiComponent } from './api/administration-api.component';
import { AdvancedApiComponent } from './api/advanced-api.component';
import { BasicApiComponent } from './api/basic-api.component';

@NgModule({
  declarations: [
    AppComponent,
    ApiComponent,
    PublicApiComponent,
    AdministrationApiComponent,
    AdvancedApiComponent,
    BasicApiComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    MaterialModule,
    HttpClientModule,
    OAuthModule.forRoot(authModuleConfig),
    RouterModule.forRoot([
      { path: 'publicapi', component: PublicApiComponent },
      // { path: 'adminapi', component: AdministrationApiComponent, canActivate: [AuthGuardAdministrationApi] },
      // { path: 'advancedapi', component: AdvancedApiComponent, canActivate: [AuthGuardAdvancedApi] },
      // { path: 'basicapi', component: BasicApiComponent, canActivate: [AuthGuardBasicApi] },
      { path: 'adminapi', component: AdministrationApiComponent, canActivate: [AuthGuard] },
      { path: 'advancedapi', component: AdvancedApiComponent, canActivate: [AuthGuard] },
      { path: 'basicapi', component: BasicApiComponent, canActivate: [AuthGuard] },
      { path: '**', redirectTo: '/', pathMatch: 'full' }
    ])
  ],
  providers: [
    // can be configured here instead of auth/auth.service
    { provide: AuthConfig, useValue: authConfig },
    { provide: OAuthModuleConfig, useValue: authModuleConfig },
    { provide: ValidationHandler, useClass: JwksValidationHandler },
    { provide: OAuthStorage, useValue: localStorage },

    AuthService,
    AuthGuard,
    AuthGuardAdministrationApi,
    AuthGuardAdvancedApi,
    AuthGuardBasicApi,
    ApiService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
