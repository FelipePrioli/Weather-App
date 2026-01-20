import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, tap, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthUser } from '../models/user.model';

interface LoginResponse {
  token: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = 'http://localhost:5037/api/auth'; // ajuste conforme seu backend

  // Observable para expor o usuário logado
  private userSubject = new BehaviorSubject<AuthUser | null>(null);
  user$ = this.userSubject.asObservable();

  constructor(private http: HttpClient) {
    this.loadUserFromToken();
  }

  // LOGIN
  login(email: string, senha: string) {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, { email, senha }).pipe(
      tap((response) => {
        localStorage.setItem('token', response.token);
        const user = this.decodeUserFromToken(response.token);
        this.userSubject.next(user);
      }),
      catchError(this.handleError),
    );
  }

  // LOGOUT
  logout() {
    localStorage.removeItem('token');
    this.userSubject.next(null);
  }

  // VERIFICA SE ESTÁ LOGADO
  isLogged(): boolean {
    return !!localStorage.getItem('token');
  }

  // RETORNA O TOKEN
  getToken(): string | null {
    return localStorage.getItem('token');
  }

  // RETORNA O USUÁRIO LOGADO ATUAL
  get currentUser(): AuthUser | null {
    return this.userSubject.value;
  }

  // DECODIFICA O JWT PARA PEGAR INFORMAÇÕES DO USUÁRIO
  private decodeUserFromToken(token: string): AuthUser {
    const payload = JSON.parse(atob(token.split('.')[1]));

    return {
      id: Number(payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']),
      nome: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
      email: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
    };
  }

  // CARREGA O USUÁRIO LOGADO AO INICIAR O APP
  private loadUserFromToken() {
    const token = localStorage.getItem('token');
    if (!token) return;

    try {
      const user = this.decodeUserFromToken(token);
      this.userSubject.next(user);
    } catch {
      this.logout(); // token inválido
    }
  }

  // TRATAMENTO CENTRAL DE ERROS HTTP
  private handleError(err: HttpErrorResponse) {
    console.error('Erro AuthService:', err);
    return throwError(() => err);
  }
}
