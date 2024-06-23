namespace VillaUtility;
public static class SD
{
    public enum ApiType 
    {
        GET,
        POST,
        PUT,
        DELETE,
    }
    public static class Role
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public static string[] GetRoles() => new string[] { Admin, User };
    }
    public static string SessionTokenKey = "JWTToken";
}
