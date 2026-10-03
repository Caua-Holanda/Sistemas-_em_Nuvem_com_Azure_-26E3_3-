import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { PagedResponse, Product, ProductPayload } from '../models/product.model';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly http = inject(HttpClient);
  private readonly endpoint = `${environment.apiUrl}/products`;

  getAll(search = ''): Observable<PagedResponse<Product>> {
    let params = new HttpParams().set('page', 1).set('pageSize', 100);
    if (search.trim()) {
      params = params.set('search', search.trim());
    }

    return this.http.get<PagedResponse<Product>>(this.endpoint, { params });
  }

  create(product: ProductPayload): Observable<Product> {
    return this.http.post<Product>(this.endpoint, product);
  }

  update(id: string, product: ProductPayload): Observable<Product> {
    return this.http.put<Product>(`${this.endpoint}/${id}`, product);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.endpoint}/${id}`);
  }
}
