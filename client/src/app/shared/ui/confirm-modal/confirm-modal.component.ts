import
{
  AfterViewInit,
  Component,
  ElementRef,
  OnChanges,
  SimpleChanges,
  ViewChild,
  input,
  output,
} from '@angular/core';

@Component({
  selector: 'app-confirm-modal',
  standalone: true,
  templateUrl: './confirm-modal.component.html',
  styleUrl: './confirm-modal.component.css',
})
export class ConfirmModalComponent implements AfterViewInit, OnChanges
{
  readonly open = input.required<boolean>();
  readonly title = input<string>('Potwierdź');
  readonly message = input<string>('Czy na pewno?');
  readonly confirmLabel = input<string>('Tak');
  readonly cancelLabel = input<string>('Anuluj');
  readonly danger = input<boolean>(false);

  readonly confirmed = output<void>();
  readonly cancelled = output<void>();

  @ViewChild('dialogRef') dialogRef!: ElementRef<HTMLDialogElement>;

  ngAfterViewInit(): void
  {
    this.sync();
  }

  ngOnChanges(_: SimpleChanges): void
  {
    this.sync();
  }

  onConfirm(): void
  {
    this.confirmed.emit();
  }

  onCancel(): void
  {
    this.cancelled.emit();
  }

  private sync(): void
  {
    const dialog = this.dialogRef?.nativeElement;
    if (!dialog) return;
    if (this.open() && !dialog.open)
    {
      dialog.showModal();
    } else if (!this.open() && dialog.open)
    {
      dialog.close();
    }
  }
}
