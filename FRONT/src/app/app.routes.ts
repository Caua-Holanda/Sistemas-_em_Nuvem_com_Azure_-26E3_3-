import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/products/products').then((component) => component.Products),
    title: 'Produtos | TechStore Cloud',
  },
  { path: '**', redirectTo: '' },
];
