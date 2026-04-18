import { Component, inject } from '@angular/core';
import { ToastService } from './toast.service';

@Component({
  selector: 'app-toast',
  standalone: true,
  templateUrl: './toast.component.html',
  styleUrl: './toast.component.css',
})
export class ToastComponent {
  private readonly service = inject(ToastService);
  readonly toasts = this.service.toasts;

  dismiss(id: number): void {
    this.service.dismiss(id);
  }
}
