import { UserResponse } from "./Auth";

export interface UrlResponse {
  id: string;
  urlOriginal: string;
  urlShortened: string;
  userId: string;
}

export interface UrlInfoResponse {
  id: string;
  urlOriginal: string;
  urlShortened: string;
  createdAt: string;
  user: UserResponse;
}
