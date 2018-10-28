import { Component } from '@angular/core';

@Component({
  selector: 'app-basic-api',
  template: 'This component is only availvable when <strong>(api_level: basic_api)</strong> claim is present.<p><app-api></app-api></p>'
})
export class BasicApiComponent {
}
