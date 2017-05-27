using System.Web.Http;
using WebApp.Integration;

namespace WebApp
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var t = new ZsjGeo().Load(new [] { 127752 });

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
