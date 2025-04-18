export abstract class AuthRoutes {
  static readonly base = `identity`;

  static readonly signup = `${this.base}/signup`;
  static readonly login = `${this.base}/login`;
  static readonly refreshToken = `${this.base}/refresh`;
  static readonly me = `${this.base}/me`;
  static readonly logout = `${this.base}/logout`;
}

export abstract class UrlRoutes {
  static readonly base = `urls`;

  static readonly about = `${this.base}/about`;
}
