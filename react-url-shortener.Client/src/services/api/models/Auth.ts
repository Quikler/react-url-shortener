export interface LoginRequest {
  username: string;
  password: string;
}

export interface SignupRequest {
  username: string;
  password: string;
  confirmPassword: string;
}

export interface AuthSuccessResponse {
  token: string;
  user: UserResponse;
  roles: string[];
}

export interface UserResponse {
  id: string;
  username: string;
}

export enum Roles {
  Admin = "Admin"
}