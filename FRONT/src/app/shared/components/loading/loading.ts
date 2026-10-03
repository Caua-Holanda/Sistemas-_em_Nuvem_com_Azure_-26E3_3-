import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'app-loading',
  standalone: true,
  template: `
    <div class="loading" role="status" aria-live="polite">
      <span class="loading__spinner" aria-hidden="true"></span>
      <span>Carregando produtos...</span>
    </div>
  `,
  styleUrl: './loading.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Loading {}
