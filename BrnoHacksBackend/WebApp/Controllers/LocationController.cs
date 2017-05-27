using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using WebApp.Helpers;
using WebApp.Integration;

namespace WebApp.Controllers
{
    public class LocationController : ApiController
    {
        private O2Api _o2Api = new O2Api();

        public IHttpActionResult GetStats(int ageGroup, int hour)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var lst = new List<object>();

            foreach (var locationId in LocationHelper.LocationIds)
            {
                var result = _o2Api.GetCountOfPeople(locationId, ageGroup, 1, hour);

                lst.Add(new { count = result, locationId });
            }

            return Ok(lst);
        }

        public IHttpActionResult GetCoords()
        {
            var zsjGeoPath = HostingEnvironment.MapPath(ConfigurationHelper.ZsjGeoPath);

            var t = new ZsjGeo().Load(zsjGeoPath, LocationHelper.LocationIds);

            return Ok(t);
        }
    }
}
