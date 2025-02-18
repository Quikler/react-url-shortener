namespace WebApi.Contracts;

public static class ApiRoutes
{
    public const string ROOT = "api";
    public const string VERSION = "v1";

    public const string BASE = $"{ROOT}/{VERSION}";

    public static class Urls
    {
        public const string GetInfo = BASE + "/urls/{urlId}";
        public const string GetAll = BASE + "/urls";
        public const string Create = BASE + "/urls";
        public const string Delete = BASE + "/urls/{urlId}";
    }

    public static class Identity
    {
        public const string Login = BASE + "/identity/login";
        public const string Signup = BASE + "/identity/signup";
        public const string Logout = BASE + "/identity/logout";
        public const string Me = BASE + "/identity/me";
        public const string Refresh = BASE + "/identity/refresh";
    }

    public static class About
    {
        public const string Get = BASE + "/about";
        public const string Update = BASE + "/about";
    }
}