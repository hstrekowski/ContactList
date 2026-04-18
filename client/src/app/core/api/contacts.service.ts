import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ContactDetail,
  ContactListItem,
  CreateContactRequest,
  UpdateContactRequest,
} from '../models/contact';

@Injectable({ providedIn: 'root' })
export class ContactsService
{
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/contacts`;

  list(): Observable<ContactListItem[]>
  {
    return this.http.get<ContactListItem[]>(this.baseUrl);
  }

  getById(id: string): Observable<ContactDetail>
  {
    return this.http.get<ContactDetail>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateContactRequest): Observable<string>
  {
    return this.http.post<string>(this.baseUrl, request);
  }

  update(id: string, request: UpdateContactRequest): Observable<void>
  {
    return this.http.put<void>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: string): Observable<void>
  {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
