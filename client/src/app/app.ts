import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { AuthStateService } from './core/auth/auth-state.service';
import { AuthService } from './core/api/auth.service';
import { ToastComponent } from './shared/ui/toast/toast.component';
import { SpinnerComponent } from './shared/ui/spinner/spinner.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, ToastComponent, SpinnerComponent],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App
{
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly authState = inject(AuthStateService);

  onLogout(): void
  {
    this.authService.logout();
    this.router.navigateByUrl('/contacts');
  }
}
