import
{
  AfterViewInit,
  Component,
  ElementRef,
  OnChanges,
  SimpleChanges,
  ViewChild,
  inject,
  input,
  output,
  signal,
} from '@angular/core';
import { ContactsService } from '../../core/api/contacts.service';
import { ContactDetail } from '../../core/models/contact';

@Component({
  selector: 'app-contact-details-modal',
  standalone: true,
  templateUrl: './contact-details-modal.component.html',
  styleUrl: './contact-details-modal.component.css',
})
export class ContactDetailsModalComponent implements AfterViewInit, OnChanges
{
  private readonly contactsService = inject(ContactsService);

  readonly contactId = input<string | null>(null);

  readonly closed = output<void>();

  readonly contact = signal<ContactDetail | null>(null);
  readonly loading = signal(false);

  @ViewChild('dialogRef') dialogRef!: ElementRef<HTMLDialogElement>;

  ngAfterViewInit(): void
  {
    this.sync();
  }

  ngOnChanges(changes: SimpleChanges): void
  {
    if (changes['contactId'])
    {
      const id = this.contactId();
      if (id)
      {
        this.load(id);
      }
      else
      {
        this.contact.set(null);
      }
    }
    this.sync();
  }

  onClose(): void
  {
    this.closed.emit();
  }

  private load(id: string): void
  {
    this.loading.set(true);
    this.contact.set(null);
    this.contactsService.getById(id).subscribe({
      next: (detail) =>
      {
        this.contact.set(detail);
        this.loading.set(false);
      },
      error: () =>
      {
        this.loading.set(false);
        this.closed.emit();
      },
    });
  }

  private sync(): void
  {
    const dialog = this.dialogRef?.nativeElement;
    if (!dialog) return;
    const shouldBeOpen = this.contactId() !== null;
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
