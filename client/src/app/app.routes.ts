import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'contacts' },
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login.page').then((m) => m.LoginPage),
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/register.page').then((m) => m.RegisterPage),
  },
  {
    path: 'contacts',
    loadComponent: () =>
      import('./features/contacts/contacts-list.page').then(
        (m) => m.ContactsListPage,
      ),
  },
  { path: '**', redirectTo: 'contacts' },
];
