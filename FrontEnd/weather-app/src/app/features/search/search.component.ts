import { Component, ChangeDetectorRef, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize, Observable, forkJoin } from 'rxjs';

import { WeatherService } from '../../core/services/weather.service';
import { AuthService } from '../../core/services/auth.service';
import { UserService } from '../../core/services/user.service';
import { FavoritesService } from '../../core/services/favorites.service';

import { Weather, Forecast } from '../../core/models/weather.model';
import { AuthUser } from '../../core/models/user.model';

// ⭐ IMPORT DO FAVORITES
import { FavoritesComponent } from '../favorites/favorites.component';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [CommonModule, FormsModule, FavoritesComponent],
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss'],
})
export class SearchComponent implements OnInit {
  ngOnInit(): void {
    this.user$.subscribe((user) => {
      if (!user) {
        // 🔒 SEM USUÁRIO → VOLTA PARA PESQUISA
        this.showFavorites = false;
        this.showForecast = false;
      }
    });
  }

  // 🔍 WEATHER
  city = '';
  weather: Weather | null = null;
  forecast: Forecast[] = [];
  loading = false;

  // 🔐 AUTH
  showLogin = false;
  showRegister = false;
  showForecast = false;

  // ⭐ FAVORITES
  showFavorites = false;

  nome = '';
  email = '';
  senha = '';
  authError = '';
  authSuccess = '';

  // ✅ Observable do usuário logado
  user$: Observable<AuthUser | null>;

  constructor(
    private weatherService: WeatherService,
    public authService: AuthService,
    private userService: UserService,
    private favoritesService: FavoritesService,
    private cdr: ChangeDetectorRef,
  ) {
    this.user$ = this.authService.user$;
  }

  onFavoriteSelected(cityName: string) {
    this.city = cityName; // atualiza o input de cidade
    this.showFavorites = false; // fecha a tela de favoritos
    this.search(); // faz a pesquisa automática
  }

  // 🔍 BUSCA DE CLIMA
  search() {
    if (!this.city.trim()) return;

    this.loading = true;
    this.weather = null;
    this.forecast = [];
    this.showForecast = false;

    forkJoin({
      weather: this.weatherService.getWeatherByCity(this.city),
      forecast: this.weatherService.getForecast(this.city),
    })
      .pipe(
        finalize(() => {
          this.loading = false;
          this.cdr.markForCheck(); // 👈 ESSENCIAL
        }),
      )
      .subscribe({
        next: ({ weather, forecast }) => {
          this.weather = weather;
          this.forecast = forecast;

          this.cdr.markForCheck(); // 👈 ESSENCIAL
        },
        error: (err) => {
          console.error('Erro ao buscar clima:', err);
          this.cdr.markForCheck();
        },
      });
  }

  // ❤️ ADICIONAR AOS FAVORITOS
  addToFavorites() {
    if (!this.weather?.city) return;

    // ❌ Usuário não logado
    if (!this.authService.isLogged()) {
      this.authError = 'Faça login ou registre-se para adicionar favoritos ❤️';
      this.cdr.detectChanges(); // força a atualização da view
      setTimeout(() => (this.authError = ''), 3000);
      return;
    }

    // Limpa mensagens anteriores
    this.authError = '';
    this.authSuccess = '';

    this.favoritesService.addFavorite(this.weather.city).subscribe({
      next: () => {
        this.authSuccess = 'Cidade adicionada aos favoritos ⭐';
        this.cdr.detectChanges(); // força a atualização da view
        setTimeout(() => (this.authSuccess = ''), 3000);
      },
      error: (err) => {
        // ✅ Trata duplicidade
        if (err.status === 409) {
          this.authError = err.error?.message || 'Esta cidade já está nos seus favoritos ❤️';
        } else {
          this.authError = 'Erro ao adicionar cidade aos favoritos ❌';
        }
        this.cdr.detectChanges(); // força a atualização da view
        setTimeout(() => (this.authError = ''), 3000);
      },
    });
  }

  // LOGIN
  login() {
    this.authError = '';
    this.authSuccess = '';

    const email = this.email.trim();
    const senha = this.senha.trim();

    if (!email || !senha) {
      this.authError = 'Informe e-mail e senha';
      return;
    }

    this.loading = true;

    this.authService
      .login(email, senha)
      .pipe(finalize(() => (this.loading = false))) // garante que o spinner sempre desliga
      .subscribe({
        next: () => {
          // 🔹 login ok
          this.showLogin = false; // fecha modal
          this.authSuccess = 'Login realizado com sucesso!';
          this.resetForm();

          setTimeout(() => (this.authSuccess = ''), 3000);
        },
        error: (err) => {
          // 🔹 login falhou
          this.showLogin = false; // fecha modal para exibir mensagem fora
          this.authError = err?.error?.message || 'E-mail ou senha inválidos';
          this.cdr.detectChanges(); // força atualização imediata da view
        },
      });
  }

  // método para resetar formulário e estado
  resetForm() {
    this.email = '';
    this.senha = '';
    this.city = '';
    this.weather = null;
    this.forecast = [];
    this.showForecast = false;
    this.showFavorites = false;
  }

  // Função de validação de e-mail
  isValidEmail(email: string): boolean {
    // Regex garante formato completo: nome@domínio.com
    const re = /^[\w-\.]+@([\w-]+\.)+[\w-]{2,}$/;
    return re.test(email);
  }

  logout() {
    this.showFavorites = false;
    this.showForecast = false;

    this.authService.logout();
  }

  // REGISTRO
  register() {
    this.authError = '';
    this.authSuccess = '';

    const nome = this.nome.trim();
    const email = this.email.trim();
    const senha = this.senha.trim();

    // ✅ Campos obrigatórios
    if (!nome || !email || !senha) {
      this.authError = 'Informe nome, e-mail e senha';
      this.showRegister = false; // fecha modal
      this.cdr.detectChanges(); // garante atualização imediata da view
      return;
    }

    // ✅ Validação de e-mail
    if (!this.isValidEmail(email)) {
      this.authError = 'Informe um e-mail válido (ex: usuario@email.com)';
      this.showRegister = false; // fecha modal
      this.cdr.detectChanges();
      return;
    }

    // ❌ Envia para o backend apenas se passou na validação
    this.userService.createUser(nome, email, senha).subscribe({
      next: () => {
        this.showRegister = false; // fecha modal
        this.nome = '';
        this.email = '';
        this.senha = '';
        this.authSuccess = 'Usuário criado com sucesso!';
        this.cdr.detectChanges();
        setTimeout(() => (this.authSuccess = ''), 3000);
      },
      error: (err) => {
        this.authError = err.error?.message || 'Erro ao criar usuário';
        this.showRegister = false; // fecha modal em caso de erro do backend
        this.cdr.detectChanges();
      },
    });
  }

  // ⭐ FAVORITES
  goToFavorites() {
    this.showFavorites = true;
  }

  backToSearch() {
    this.showFavorites = false;
  }

  toggleForecast() {
    this.showForecast = !this.showForecast;
  }

  getTempClass(temp: number): string {
    if (temp <= 10) return 'cold';
    if (temp <= 25) return 'mild';
    if (temp <= 30) return 'warm';
    return 'hot';
  }

  trackByDate(index: number, day: Forecast): string {
    return day.date;
  }
}
