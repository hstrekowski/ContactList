import
{
  AfterViewInit,
  Component,
  ElementRef,
  OnChanges,
  OnDestroy,
  SimpleChanges,
  ViewChild,
  inject,
  input,
  output,
  signal,
} from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { Subscription } from 'rxjs';
import { ContactsService } from '../../core/api/contacts.service';
import { CategoriesService } from '../../core/api/categories.service';
import { ToastService } from '../../shared/ui/toast/toast.service';
import
{
  CATEGORY_NAMES,
  Category,
  Subcategory,
} from '../../core/models/category';
import
{
  CreateContactRequest,
  UpdateContactRequest,
} from '../../core/models/contact';
import { passwordComplexityValidator } from '../../shared/validators/password.validator';

type FormMode = 'create' | 'edit' | null;

@Component({
  selector: 'app-contact-form-modal',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './contact-form-modal.component.html',
  styleUrl: './contact-form-modal.component.css',
})
export class ContactFormModalComponent
  implements AfterViewInit, OnChanges, OnDestroy
{
  private readonly fb = inject(FormBuilder);
  private readonly contactsService = inject(ContactsService);
  private readonly categoriesService = inject(CategoriesService);
  private readonly toast = inject(ToastService);

  readonly mode = input<FormMode>(null);
  readonly contactId = input<string | null>(null);

  readonly saved = output<void>();
  readonly closed = output<void>();

  readonly submitting = signal(false);
  readonly loadingContact = signal(false);
  readonly error = signal<string | null>(null);
  readonly categories = signal<Category[]>([]);
  readonly subcategories = signal<Subcategory[]>([]);

  @ViewChild('dialogRef') dialogRef!: ElementRef<HTMLDialogElement>;

  readonly form = this.fb.nonNullable.group({
    firstName: ['', [Validators.required, Validators.maxLength(100)]],
    lastName: ['', [Validators.required, Validators.maxLength(100)]],
    email: ['', [Validators.required, Validators.email]],
    password: [''],
    phoneNumber: ['', [Validators.required, Validators.maxLength(20)]],
    dateOfBirth: ['', [Validators.required]],
    categoryId: ['', [Validators.required]],
    subcategoryId: [''],
    subcategoryName: [''],
  });

  private categorySub?: Subscription;

  constructor()
  {
    this.categoriesService.list().subscribe((cats) => this.categories.set(cats));

    this.categorySub = this.form.controls.categoryId.valueChanges.subscribe(
      (id) => this.onCategoryChange(id),
    );
  }

  ngAfterViewInit(): void
  {
    this.sync();
  }

  ngOnChanges(changes: SimpleChanges): void
  {
    if (changes['mode'] || changes['contactId'])
    {
      const mode = this.mode();
      if (mode === 'create')
      {
        this.resetForCreate();
      }
      else if (mode === 'edit' && this.contactId())
      {
        this.loadForEdit(this.contactId()!);
      }
    }
    this.sync();
  }

  ngOnDestroy(): void
  {
    this.categorySub?.unsubscribe();
  }

  get selectedCategoryName(): string | null
  {
    const id = this.form.controls.categoryId.value;
    return this.categories().find((c) => c.id === id)?.name ?? null;
  }

  get showSubcategorySelect(): boolean
  {
    return this.selectedCategoryName === CATEGORY_NAMES.business;
  }

  get showSubcategoryText(): boolean
  {
    return this.selectedCategoryName === CATEGORY_NAMES.other;
  }

  get passwordErrors(): Record<string, string> | null
  {
    const errors = this.form.controls.password.errors;
    return errors ? (errors as Record<string, string>) : null;
  }

  onClose(): void
  {
    this.closed.emit();
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

    const raw = this.form.getRawValue();
    const categoryName = this.selectedCategoryName;

    let subcategoryId: string | null = null;
    let subcategoryName: string | null = null;

    if (categoryName === CATEGORY_NAMES.business)
    {
      subcategoryId = raw.subcategoryId || null;
    }
    else if (categoryName === CATEGORY_NAMES.other)
    {
      subcategoryName = raw.subcategoryName || null;
    }

    const base = {
      firstName: raw.firstName,
      lastName: raw.lastName,
      email: raw.email,
      phoneNumber: raw.phoneNumber,
      dateOfBirth: raw.dateOfBirth,
      categoryId: raw.categoryId,
      subcategoryId,
      subcategoryName,
    };

    const mode = this.mode();
    if (mode === 'create')
    {
      const request: CreateContactRequest = { ...base, password: raw.password };
      this.contactsService.create(request).subscribe({
        next: () =>
        {
          this.submitting.set(false);
          this.toast.success('Kontakt dodany.');
          this.saved.emit();
        },
        error: (err: HttpErrorResponse) => this.handleError(err),
      });
    }
    else if (mode === 'edit')
    {
      const id = this.contactId();
      if (!id)
      {
        this.submitting.set(false);
        return;
      }
      const request: UpdateContactRequest = {
        ...base,
        password: raw.password === '' ? null : raw.password,
      };
      this.contactsService.update(id, request).subscribe({
        next: () =>
        {
          this.submitting.set(false);
          this.toast.success('Kontakt zaktualizowany.');
          this.saved.emit();
        },
        error: (err: HttpErrorResponse) => this.handleError(err),
      });
    }
  }

  private onCategoryChange(categoryId: string): void
  {
    const cat = this.categories().find((c) => c.id === categoryId);
    const subIdCtrl = this.form.controls.subcategoryId;
    const subNameCtrl = this.form.controls.subcategoryName;

    if (!cat)
    {
      this.subcategories.set([]);
      subIdCtrl.clearValidators();
      subNameCtrl.clearValidators();
      subIdCtrl.setValue('', { emitEvent: false });
      subNameCtrl.setValue('', { emitEvent: false });
    }
    else if (cat.name === CATEGORY_NAMES.business)
    {
      subIdCtrl.setValidators([Validators.required]);
      subNameCtrl.clearValidators();
      subNameCtrl.setValue('', { emitEvent: false });
      this.categoriesService
        .getSubcategories(categoryId)
        .subscribe((subs) => this.subcategories.set(subs));
    }
    else if (cat.name === CATEGORY_NAMES.other)
    {
      subIdCtrl.clearValidators();
      subIdCtrl.setValue('', { emitEvent: false });
      this.subcategories.set([]);
      subNameCtrl.setValidators([
        Validators.required,
        Validators.maxLength(100),
      ]);
    }
    else
    {
      subIdCtrl.clearValidators();
      subNameCtrl.clearValidators();
      subIdCtrl.setValue('', { emitEvent: false });
      subNameCtrl.setValue('', { emitEvent: false });
      this.subcategories.set([]);
    }

    subIdCtrl.updateValueAndValidity({ emitEvent: false });
    subNameCtrl.updateValueAndValidity({ emitEvent: false });
  }

  private resetForCreate(): void
  {
    this.error.set(null);
    this.submitting.set(false);
    this.subcategories.set([]);
    this.form.reset();
    this.form.controls.password.setValidators([
      Validators.required,
      passwordComplexityValidator(),
    ]);
    this.form.controls.password.updateValueAndValidity({ emitEvent: false });
  }

  private loadForEdit(id: string): void
  {
    this.error.set(null);
    this.submitting.set(false);
    this.loadingContact.set(true);
    this.form.controls.password.setValidators([passwordComplexityValidator()]);
    this.form.controls.password.updateValueAndValidity({ emitEvent: false });

    this.contactsService.getById(id).subscribe({
      next: (c) =>
      {
        this.form.patchValue({
          firstName: c.firstName,
          lastName: c.lastName,
          email: c.email,
          password: '',
          phoneNumber: c.phoneNumber,
          dateOfBirth: c.dateOfBirth,
          categoryId: c.categoryId,
          subcategoryId: c.subcategoryId ?? '',
          subcategoryName: c.subcategoryName ?? '',
        });
        this.loadingContact.set(false);
      },
      error: () =>
      {
        this.loadingContact.set(false);
        this.closed.emit();
      },
    });
  }

  private handleError(err: HttpErrorResponse): void
  {
    this.submitting.set(false);
    if (err.status === 409)
    {
      this.form.controls.email.setErrors({ duplicate: true });
      this.error.set('Kontakt z tym adresem email już istnieje.');
      return;
    }
    if (err.status === 400)
    {
      const body = err.error as
        | { detail?: string; errors?: Record<string, string[]> }
        | null;
      if (body?.errors && Object.keys(body.errors).length > 0)
      {
        const firstKey = Object.keys(body.errors)[0];
        const firstMsg = body.errors[firstKey]?.[0];
        this.error.set(firstMsg ?? 'Popraw błędy w formularzu.');
      }
      else if (body?.detail)
      {
        this.error.set(body.detail);
      }
      else
      {
        this.error.set('Popraw błędy w formularzu.');
      }
      return;
    }
    this.error.set('Nie udało się zapisać. Spróbuj ponownie.');
  }

  private sync(): void
  {
    const dialog = this.dialogRef?.nativeElement;
    if (!dialog) return;
    const shouldBeOpen = this.mode() !== null;
    if (shouldBeOpen && !dialog.open)
    {
      dialog.showModal();
    }
    else if (!shouldBeOpen && dialog.open)
    {
      dialog.close();
    }
  }
}
