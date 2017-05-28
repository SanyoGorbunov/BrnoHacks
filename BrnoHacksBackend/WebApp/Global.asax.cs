using System.Web.Http;
using WebApp.Helpers;
using WebApp.Integration;

namespace WebApp
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
