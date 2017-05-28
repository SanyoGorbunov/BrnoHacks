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

        public static string CsvGeoPath
        {
            get { return ConfigurationManager.AppSettings["csvgeopath"]; }
        }

        public static string O2SampleAge
        {
            get { return ConfigurationManager.AppSettings["02sampleage"]; }
        }

        public static string O2SampleGender
        {
            get { return ConfigurationManager.AppSettings["02samplegender"]; }
        }

        public static string VehiclesPath
        {
            get
            {
                return ConfigurationManager.AppSettings["vehiclespath"];
            }
        }
    }
}