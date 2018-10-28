import { Component } from '@angular/core';

@Component({
  selector: 'app-administration-api',
  template: 'This component is only availvable when <strong>(api_level: administration_api)</strong> claim is present.<p><app-api></app-api></p>'
})
export class AdministrationApiComponent {
}
