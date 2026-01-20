import { Component, OnInit, ChangeDetectorRef, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';

import { FavoritesService } from '../../core/services/favorites.service';
import { Favorite } from '../../core/models/favorite.model';

@Component({
  selector: 'app-favorites',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './favorites.component.html',
  styleUrls: ['./favorites.component.scss'],
})
export class FavoritesComponent implements OnInit {
  favorites: Favorite[] = [];
  highlightedId: number | null = null;

  loading = false;
  saving = false;

  newCity = '';

  successMessage = '';
  errorMessage = '';

  // ✨ Evento emitido quando um favorito é clicado
  @Output() favoriteClicked = new EventEmitter<string>();

  constructor(
    private favoritesService: FavoritesService,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit() {
    this.loadFavorites();
  }

  /** ==========================
   * CARREGAR FAVORITOS
   * ========================== */
  loadFavorites() {
    this.loading = true;
    this.errorMessage = '';

    this.favoritesService
      .getFavorites()
      .pipe(
        finalize(() => {
          this.loading = false;
          this.cdr.detectChanges();
        }),
      )
      .subscribe({
        next: (favorites) => {
          this.favorites = favorites ?? [];
        },
        error: () => {
          this.showError('Erro ao carregar favoritos 😢');
          this.favorites = [];
        },
      });
  }

  /** ==========================
   * ADICIONAR FAVORITO
   * ========================== */
  addFavorite() {
    const city = this.newCity.trim();
    if (!city || this.saving) return;

    this.saving = true;
    this.clearMessages();

    this.favoritesService
      .addFavorite(city)
      .pipe(
        finalize(() => {
          this.saving = false;
          this.cdr.detectChanges();
        }),
      )
      .subscribe({
        next: () => {
          this.newCity = '';
          this.loadFavorites(); // atualiza lista
          this.showSuccess(`Cidade ${city} adicionada aos favoritos ⭐`);
        },
        error: (err) => {
          this.showError(err?.error?.message || 'Esta cidade já está nos favoritos 😢');
        },
      });
  }

  /** ==========================
   * REMOVER FAVORITO
   * ========================== */
  removeFavorite(favorite: Favorite, event?: Event) {
    if (event) event.stopPropagation(); // evita que clique no item dispare pesquisa
    if (this.saving) return;
    this.saving = true;
    this.clearMessages();

    this.favoritesService
      .removeFavorite(favorite.id)
      .pipe(
        finalize(() => {
          this.saving = false;
          this.cdr.detectChanges();
        }),
      )
      .subscribe({
        next: () => {
          // Atualiza lista local
          this.favorites = this.favorites.filter((f) => f.id !== favorite.id);
          this.showSuccess(`${favorite.nome} removida dos favoritos ⭐`);
        },
        error: (err) => {
          if (err?.status === 404) {
            this.showError('Cidade não encontrada nos favoritos ❌');
          } else {
            this.showError('Erro ao remover favorito ❌');
          }
        },
      });
  }

  /** ==========================
   * CLIQUE EM FAVORITO
   * ========================== */
  selectFavorite(favorite: Favorite) {
    this.favoriteClicked.emit(favorite.nome); // envia o nome da cidade para o componente pai
  }

  /** ==========================
   * FUNÇÕES AUXILIARES
   * ========================== */
  highlight(favorite: Favorite) {
    this.highlightedId = favorite.id;
  }

  trackById(index: number, item: Favorite) {
    return item.id;
  }

  /** ==========================
   * MENSAGENS
   * ========================== */
  private showSuccess(message: string, duration = 3000) {
    this.successMessage = message;
    this.cdr.detectChanges();
    setTimeout(() => (this.successMessage = ''), duration);
  }

  private showError(message: string, duration = 3000) {
    this.errorMessage = message;
    this.cdr.detectChanges();
    setTimeout(() => (this.errorMessage = ''), duration);
  }

  private clearMessages() {
    this.successMessage = '';
    this.errorMessage = '';
  }
}
