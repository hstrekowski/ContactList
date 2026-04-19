import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../core/api/auth.service';
import { passwordComplexityValidator } from '../../shared/validators/password.validator';

@Component({
  selector: 'app-register-page',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.page.html',
  styleUrl: './register.page.css',
})
export class RegisterPage
{
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly error = signal<string | null>(null);
  readonly submitting = signal(false);

  readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: [
      '',
      [Validators.required, passwordComplexityValidator()],
    ],
  });

  get passwordErrors(): Record<string, string> | null
  {
    const errors = this.form.controls.password.errors;
    if (!errors)
    {
      return null;
    }
    return errors as Record<string, string>;
  }

  onSubmit(): void
  {
    if (this.form.invalid || this.submitting())
    {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.error.set(null);

    this.authService.register(this.form.getRawValue()).subscribe({
      next: () =>
      {
        this.router.navigateByUrl('/contacts');
      },
      error: (err: HttpErrorResponse) =>
      {
        this.submitting.set(false);
        if (err.status === 409)
        {
          this.error.set('Konto z tym adresem email już istnieje.');
        }
        else if (err.status === 400)
        {
          this.error.set('Popraw błędy w formularzu.');
        }
        else
        {
          this.error.set('Nie udało się zarejestrować. Spróbuj ponownie.');
        }
      },
    });
  }
}
