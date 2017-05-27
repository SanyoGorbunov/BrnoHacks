using System.Configuration;

namespace WebApp.Helpers
{
    public static class ConfigurationHelper
    {
        public static string ApiKey
        {
            get { return ConfigurationManager.AppSettings["apikey"]; }
        }

        public static string O2BasePath
        {
            get { return ConfigurationManager.AppSettings["o2basepath"]; }
        }

        public static string ZsjGeoPath
        {
            get { return ConfigurationManager.AppSettings["zsjgeopath"]; }
        }
        
        public static string AuthorizationToken
        {
            get { return ConfigurationManager.AppSettings["authtoken"]; }
        }
    }
}