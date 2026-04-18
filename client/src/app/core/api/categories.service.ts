import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Category, Subcategory } from '../models/category';

@Injectable({ providedIn: 'root' })
export class CategoriesService
{
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/categories`;

  list(): Observable<Category[]>
  {
    return this.http.get<Category[]>(this.baseUrl);
  }

  getSubcategories(categoryId: string): Observable<Subcategory[]>
  {
    return this.http.get<Subcategory[]>(
      `${this.baseUrl}/${categoryId}/subcategories`,
    );
  }
}
