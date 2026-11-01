import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';
import { GroceryList } from './features/grocery-list/grocery-list';

export const routes: Routes = [
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'groceries', component: GroceryList, canActivate: [authGuard] },
  { path: '', redirectTo: 'groceries', pathMatch: 'full' },
  { path: '**', redirectTo: 'groceries' },
];
