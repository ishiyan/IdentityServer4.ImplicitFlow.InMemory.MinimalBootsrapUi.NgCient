import { Component } from '@angular/core';
import { Observable } from 'rxjs';

import { ApiService } from './api.service';

@Component({
  selector: 'app-api',
  template: `<p><button mat-raised-button color="primary" (click)='publicApi()'>Call public API</button> {{publicApiReply$ | async}}</p>
    <p><button mat-raised-button color="primary" (click)='administrationApi()'>Call administration API</button> {{administrationApiReply$ | async}}</p>
    <p><button mat-raised-button color="primary" (click)='basicApi()'>Call basic API</button> {{basicApiReply$ | async}}</p>
    <p><button mat-raised-button color="primary" (click)='advancedApi()'>Call advanced API</button> {{advancedApiReply$ | async}}</p>`
})
export class ApiComponent {
  public publicApiReply$: Observable<string>;
  public administrationApiReply$: Observable<string>;
  public basicApiReply$: Observable<string>;
  public advancedApiReply$: Observable<string>;

  constructor(private apiService: ApiService) { }

  public publicApi(): void {
    this.publicApiReply$ = this.apiService.callApi('publicapi');
  }

  public administrationApi(): void {
    this.administrationApiReply$ = this.apiService.callApi('administrationapi');
  }

  public basicApi(): void {
    this.basicApiReply$ = this.apiService.callApi('basicapi');
  }

  public advancedApi(): void {
    this.advancedApiReply$ = this.apiService.callApi('advancedapi');
  }
}
