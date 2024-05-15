namespace ResourceAllocationTool.Services
{
    public class Constants
    {

        public const string AuthCookie = "_authResourceAllocation";

        public const int ApplicationBit = 16;

        public const int CacheExpHrs = 24;

        public static class AntiForgery
        {
            public const string Header = "X-XSRF-TOKEN";
            public const string Cookie = "XSRF-TOKEN";
        }

        public static class CacheKeys
        {
            public const string Roles = "_cacheRoles";
            public const string Periods = "_cachePeriods";
            public const string Managers = "_cacheManagers";
            public const string Projects = "_cacheProjects";

        }

        public static class LdapAttributes
        {
            public const string CommonName = "cn";
            public const string LastName = "sn";
            public const string FirstName = "givenname";
        }

        public static class Roles
        {
            public const int Administator = 1;
            public const int Supervisor = 2;
        }



    }

}
