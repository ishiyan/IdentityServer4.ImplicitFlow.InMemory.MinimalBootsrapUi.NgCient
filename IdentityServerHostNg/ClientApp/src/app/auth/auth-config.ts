import { AuthConfig } from 'angular-oauth2-oidc';

export const authConfig: AuthConfig = {
  issuer: window.location.origin,
  clientId: 'ngclient',
  redirectUri: window.location.origin + '/',

  // The first two are defined by OIDC, the rest are usecase-specific
  scope: 'openid profile offline_access api_level',

  silentRefreshRedirectUri: window.location.origin + '/silent-refresh.html',
  oidc: true,
  requestAccessToken: true,
  silentRefreshTimeout: 50000, // For faster testing
  timeoutFactor: 0.75,
  sessionChecksEnabled: true,
  showDebugInformation: true // Also requires enabling "Verbose" level in devtools
};
