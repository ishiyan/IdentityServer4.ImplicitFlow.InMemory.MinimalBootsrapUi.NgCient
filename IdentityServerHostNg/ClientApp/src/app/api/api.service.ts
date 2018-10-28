import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { OAuthService } from 'angular-oauth2-oidc';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { AuthService } from '../auth/auth.service';

const apiUrl = window.location.origin + "/api/test/";

@Injectable()
export class ApiService {
  constructor(private oauthService: OAuthService, private httpClient: HttpClient, private authService: AuthService) { }

  public callApi(name: string): Observable<string> {
    if (this.oauthService.hasValidAccessToken()) {
      var headers = new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.oauthService.getAccessToken()
      });
      return this.httpClient.get<string>(apiUrl + name, { headers })
        .pipe(tap(
          data => data,
          error => this.authService.notifyError(error.message)
        ));
    }
  }
}
