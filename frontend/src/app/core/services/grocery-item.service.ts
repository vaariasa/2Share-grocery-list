import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateGroceryItemRequest, GroceryItem } from '../models/grocery-item.model';

/**
 * Calls the grocery list endpoints of the API.
 * The token is added automatically by the auth interceptor.
 */
@Injectable({ providedIn: 'root' })
export class GroceryItemService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/grocery-items`;

  getAll(): Observable<GroceryItem[]> {
    return this.http.get<GroceryItem[]>(this.apiUrl);
  }

  add(request: CreateGroceryItemRequest): Observable<GroceryItem> {
    return this.http.post<GroceryItem>(this.apiUrl, request);
  }

  remove(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  /** Tag (true) or un-tag (false) an item. */
  setInBasket(id: string, isInBasket: boolean): Observable<GroceryItem> {
    return this.http.patch<GroceryItem>(`${this.apiUrl}/${id}/basket`, { isInBasket });
  }
}
