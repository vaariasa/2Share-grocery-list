import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { GroceryItem } from '../../core/models/grocery-item.model';
import { AuthService } from '../../core/services/auth.service';
import { GroceryItemService } from '../../core/services/grocery-item.service';
import { getErrorMessage } from '../../core/utils/error-message';

/**
 * The grocery list page (user stories 3 to 8):
 * see the list, add, remove, tag / un-tag items, and log out.
 */
@Component({
  selector: 'app-grocery-list',
  imports: [ReactiveFormsModule],
  templateUrl: './grocery-list.html',
  styleUrl: './grocery-list.scss',
})
export class GroceryList implements OnInit {
  private readonly groceryItemService = inject(GroceryItemService);
  protected readonly authService = inject(AuthService);

  protected readonly items = signal<GroceryItem[]>([]);
  protected readonly isLoading = signal(true);
  protected readonly errorMessage = signal('');

  protected readonly form = inject(FormBuilder).nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    quantity: [1, [Validators.required, Validators.min(1)]],
  });

  ngOnInit(): void {
    this.loadItems();
  }

  protected addItem(): void {
    if (this.form.invalid) {
      return;
    }

    this.errorMessage.set('');
    this.groceryItemService.add(this.form.getRawValue()).subscribe({
      next: (item) => {
        this.items.update((items) => [...items, item]);
        this.form.reset();
      },
      error: (error) => this.errorMessage.set(getErrorMessage(error)),
    });
  }

  /** Tag the item when it goes into the basket, un-tag it when it goes back. */
  protected toggleBasket(item: GroceryItem): void {
    this.errorMessage.set('');
    this.groceryItemService.setInBasket(item.id, !item.isInBasket).subscribe({
      next: (updated) =>
        this.items.update((items) => items.map((i) => (i.id === updated.id ? updated : i))),
      error: (error) => {
        this.errorMessage.set(getErrorMessage(error));
        this.loadItems(); // get back in sync with the server
      },
    });
  }

  protected removeItem(item: GroceryItem): void {
    this.errorMessage.set('');
    this.groceryItemService.remove(item.id).subscribe({
      next: () => this.items.update((items) => items.filter((i) => i.id !== item.id)),
      error: (error) => this.errorMessage.set(getErrorMessage(error)),
    });
  }

  protected logout(): void {
    this.authService.logout();
  }

  private loadItems(): void {
    this.groceryItemService.getAll().subscribe({
      next: (items) => {
        this.items.set(items);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set(getErrorMessage(error));
        this.isLoading.set(false);
      },
    });
  }
}
