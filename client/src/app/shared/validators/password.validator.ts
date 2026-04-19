import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function passwordComplexityValidator(): ValidatorFn
{
  return (control: AbstractControl): ValidationErrors | null =>
  {
    const value = control.value as string | null | undefined;
    if (value === null || value === undefined || value === '')
    {
      return null;
    }

    const errors: ValidationErrors = {};

    if (value.length < 8)
    {
      errors['minlength'] = 'Hasło musi mieć co najmniej 8 znaków.';
    }
    if (value.length > 128)
    {
      errors['maxlength'] = 'Hasło nie może być dłuższe niż 128 znaków.';
    }
    if (!/[A-Z]/.test(value))
    {
      errors['uppercase'] = 'Hasło musi zawierać co najmniej jedną wielką literę.';
    }
    if (!/[a-z]/.test(value))
    {
      errors['lowercase'] = 'Hasło musi zawierać co najmniej jedną małą literę.';
    }
    if (!/[0-9]/.test(value))
    {
      errors['digit'] = 'Hasło musi zawierać co najmniej jedną cyfrę.';
    }
    if (!/[^a-zA-Z0-9]/.test(value))
    {
      errors['special'] = 'Hasło musi zawierać co najmniej jeden znak specjalny.';
    }

    return Object.keys(errors).length > 0 ? errors : null;
  };
}
