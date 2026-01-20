import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { CreateUserRequest, UserResponse } from '../models/user.model';


@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly baseUrl = 'http://localhost:5037/api/users'; // Ajuste a URL do backend

  constructor(private http: HttpClient) {}

  /**
   * Cria um novo usuário
   * @param nome Nome do usuário
   * @param email Email do usuário
   * @param senha Senha do usuário
   */
  createUser(nome: string, email: string, senha: string): Observable<UserResponse> {
    const payload: CreateUserRequest = { nome, email, senha };
    return this.http.post<UserResponse>(this.baseUrl, payload)
      .pipe(catchError(this.handleError));
  }

  /**
   * Retorna os dados do usuário logado
   */
  getMe(): Observable<UserResponse> {
    return this.http.get<UserResponse>(`${this.baseUrl}/me`)
      .pipe(catchError(this.handleError));
  }

  /**
   * Tratamento centralizado de erros HTTP
   */
  private handleError(err: HttpErrorResponse) {
    console.error('Erro HTTP:', err);
    return throwError(() => err);
  }
}
