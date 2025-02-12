namespace WebApi.Contracts;

public static class ApiRoutes
{
    public const string ROOT = "api";
    public const string VERSION = "v1";

    public const string BASE = $"{ROOT}/{VERSION}";

    public static class Urls
    {
        public const string Get = BASE + "/url/{shortCode}";
        public const string Create = BASE + "/url";
        public const string Delete = BASE + "/url/{shortCode}";
    }

    public static class Identity
    {
        public const string Login = BASE + "/identity/login";
        public const string Signup = BASE + "/identity/signup";
        public const string Logout = BASE + "/identity/logout";
        public const string Me = BASE + "/identity/me";
        public const string Refresh = BASE + "/identity/refresh";
    }
}