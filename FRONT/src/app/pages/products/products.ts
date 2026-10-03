import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize } from 'rxjs';

import { Product, ProductPayload } from '../../core/models/product.model';
import { ProductService } from '../../core/services/product.service';
import { Loading } from '../../shared/components/loading/loading';

type Feedback = { type: 'success' | 'error'; text: string };

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, Loading],
  templateUrl: './products.html',
  styleUrl: './products.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Products implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly productService = inject(ProductService);

  readonly products = signal<Product[]>([]);
  readonly totalItems = signal(0);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly deletingId = signal<string | null>(null);
  readonly editingId = signal<string | null>(null);
  readonly feedback = signal<Feedback | null>(null);

  readonly searchForm = this.formBuilder.nonNullable.group({ search: [''] });
  readonly productForm = this.formBuilder.nonNullable.group({
    name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(120)]],
    sku: [
      '',
      [
        Validators.required,
        Validators.minLength(2),
        Validators.maxLength(40),
        Validators.pattern(/^[A-Za-z0-9_-]+$/),
      ],
    ],
    description: ['', [Validators.maxLength(500)]],
    price: [0.01, [Validators.required, Validators.min(0.01)]],
    stockQuantity: [0, [Validators.required, Validators.min(0), Validators.pattern(/^\d+$/)]],
    isActive: [true],
  });

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.loading.set(true);
    const search = this.searchForm.controls.search.value;

    this.productService
      .getAll(search)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (response) => {
          this.products.set(response.items);
          this.totalItems.set(response.totalItems);
        },
        error: (error: HttpErrorResponse) => {
          this.products.set([]);
          this.totalItems.set(0);
          this.showError(
            error,
            'Não foi possível carregar os produtos. Verifique se a API está em execução.',
          );
        },
      });
  }

  clearSearch(): void {
    this.searchForm.reset({ search: '' });
    this.loadProducts();
  }

  save(): void {
    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      this.feedback.set({ type: 'error', text: 'Revise os campos destacados antes de salvar.' });
      return;
    }

    const formValue = this.productForm.getRawValue();
    const payload: ProductPayload = {
      name: formValue.name.trim(),
      sku: formValue.sku.trim().toUpperCase(),
      description: formValue.description.trim() || null,
      price: Number(formValue.price),
      stockQuantity: Number(formValue.stockQuantity),
      isActive: formValue.isActive,
    };
    const id = this.editingId();
    const request = id
      ? this.productService.update(id, payload)
      : this.productService.create(payload);

    this.saving.set(true);
    this.feedback.set(null);
    request.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: () => {
        this.feedback.set({
          type: 'success',
          text: id ? 'Produto atualizado com sucesso.' : 'Produto cadastrado com sucesso.',
        });
        this.resetForm(false);
        this.loadProducts();
      },
      error: (error: HttpErrorResponse) =>
        this.showError(error, 'Não foi possível salvar o produto.'),
    });
  }

  edit(product: Product): void {
    this.editingId.set(product.id);
    this.feedback.set(null);
    this.productForm.setValue({
      name: product.name,
      sku: product.sku,
      description: product.description ?? '',
      price: product.price,
      stockQuantity: product.stockQuantity,
      isActive: product.isActive,
    });
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  remove(product: Product): void {
    if (!window.confirm(`Deseja realmente excluir o produto "${product.name}"?`)) {
      return;
    }

    this.deletingId.set(product.id);
    this.feedback.set(null);
    this.productService
      .delete(product.id)
      .pipe(finalize(() => this.deletingId.set(null)))
      .subscribe({
        next: () => {
          if (this.editingId() === product.id) {
            this.resetForm(false);
          }
          this.feedback.set({ type: 'success', text: 'Produto excluído com sucesso.' });
          this.loadProducts();
        },
        error: (error: HttpErrorResponse) =>
          this.showError(error, 'Não foi possível excluir o produto.'),
      });
  }

  resetForm(clearFeedback = true): void {
    this.editingId.set(null);
    this.productForm.reset({
      name: '',
      sku: '',
      description: '',
      price: 0.01,
      stockQuantity: 0,
      isActive: true,
    });
    if (clearFeedback) {
      this.feedback.set(null);
    }
  }

  hasError(controlName: keyof typeof this.productForm.controls, error: string): boolean {
    const control = this.productForm.controls[controlName];
    return control.touched && control.hasError(error);
  }

  private showError(error: HttpErrorResponse, fallback: string): void {
    const validationErrors = error.error?.errors as Record<string, string[]> | undefined;
    const firstValidationError = validationErrors
      ? Object.values(validationErrors).flat()[0]
      : undefined;
    const text = firstValidationError || error.error?.detail || fallback;
    this.feedback.set({ type: 'error', text });
  }
}
