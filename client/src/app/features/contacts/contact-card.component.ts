import { Component, inject, input, output } from '@angular/core';
import { AuthStateService } from '../../core/auth/auth-state.service';
import { ContactListItem } from '../../core/models/contact';

@Component({
  selector: 'app-contact-card',
  standalone: true,
  templateUrl: './contact-card.component.html',
  styleUrl: './contact-card.component.css',
})
export class ContactCardComponent
{
  readonly contact = input.required<ContactListItem>();

  readonly open = output<string>();
  readonly edit = output<string>();
  readonly remove = output<string>();

  readonly isLoggedIn = inject(AuthStateService).isLoggedIn;

  onOpen(): void
  {
    this.open.emit(this.contact().id);
  }

  onEdit(event: MouseEvent): void
  {
    event.stopPropagation();
    this.edit.emit(this.contact().id);
  }

  onRemove(event: MouseEvent): void
  {
    event.stopPropagation();
    this.remove.emit(this.contact().id);
  }
}
