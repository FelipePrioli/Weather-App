export interface CreateUserRequest {
  nome: string;
  email: string;
  senha: string;
}

export interface UserResponse {
  id: number;
  nome: string;
  email: string;
}

export interface AuthUser {
  id: number;
  nome: string;
  email: string;
}