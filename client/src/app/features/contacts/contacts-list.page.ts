import { Component, inject, signal } from '@angular/core';
import { AuthStateService } from '../../core/auth/auth-state.service';
import { ContactsService } from '../../core/api/contacts.service';
import { ToastService } from '../../shared/ui/toast/toast.service';
import { ContactListItem } from '../../core/models/contact';
import { ContactCardComponent } from './contact-card.component';
import { ContactDetailsModalComponent } from './contact-details-modal.component';
import { ContactFormModalComponent } from './contact-form-modal.component';
import { ConfirmModalComponent } from '../../shared/ui/confirm-modal/confirm-modal.component';

type FormMode = 'create' | 'edit' | null;

@Component({
  selector: 'app-contacts-list-page',
  standalone: true,
  imports: [
    ContactCardComponent,
    ContactDetailsModalComponent,
    ContactFormModalComponent,
    ConfirmModalComponent,
  ],
  templateUrl: './contacts-list.page.html',
  styleUrl: './contacts-list.page.css',
})
export class ContactsListPage
{
  private readonly contactsService = inject(ContactsService);
  private readonly toast = inject(ToastService);
  readonly authState = inject(AuthStateService);

  readonly contacts = signal<ContactListItem[]>([]);
  readonly loading = signal(true);

  readonly selectedDetailId = signal<string | null>(null);
  readonly formMode = signal<FormMode>(null);
  readonly editingContactId = signal<string | null>(null);
  readonly deleteCandidate = signal<ContactListItem | null>(null);

  constructor()
  {
    this.refresh();
  }

  refresh(): void
  {
    this.loading.set(true);
    this.contactsService.list().subscribe({
      next: (items) =>
      {
        this.contacts.set(items);
        this.loading.set(false);
      },
      error: () =>
      {
        this.loading.set(false);
      },
    });
  }

  onOpenDetails(id: string): void
  {
    this.selectedDetailId.set(id);
  }

  onCloseDetails(): void
  {
    this.selectedDetailId.set(null);
  }

  onCreate(): void
  {
    this.editingContactId.set(null);
    this.formMode.set('create');
  }

  onEdit(id: string): void
  {
    this.editingContactId.set(id);
    this.formMode.set('edit');
    this.selectedDetailId.set(null);
  }

  onCloseForm(): void
  {
    this.formMode.set(null);
    this.editingContactId.set(null);
  }

  onFormSaved(): void
  {
    this.onCloseForm();
    this.refresh();
  }

  onDeleteRequest(id: string): void
  {
    const candidate = this.contacts().find((c) => c.id === id);
    if (candidate)
    {
      this.deleteCandidate.set(candidate);
    }
  }

  onConfirmDelete(): void
  {
    const candidate = this.deleteCandidate();
    if (!candidate)
    {
      return;
    }
    this.contactsService.delete(candidate.id).subscribe({
      next: () =>
      {
        this.toast.success('Kontakt usunięty.');
        this.deleteCandidate.set(null);
        this.selectedDetailId.set(null);
        this.refresh();
      },
      error: () =>
      {
        this.deleteCandidate.set(null);
      },
    });
  }

  onCancelDelete(): void
  {
    this.deleteCandidate.set(null);
  }
}
