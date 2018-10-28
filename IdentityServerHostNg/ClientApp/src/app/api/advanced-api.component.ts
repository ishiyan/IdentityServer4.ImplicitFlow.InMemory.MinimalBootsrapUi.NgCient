import { Component } from '@angular/core';

@Component({
  selector: 'app-advanced-api',
  template: 'This component is only availvable when <strong>(api_level: advanced_api)</strong> claim is present.<p><app-api></app-api></p>'
})
export class AdvancedApiComponent {
}
