using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using WebApp.Integration;

namespace WebApp.Controllers
{
    public class LocationController : ApiController
    {
        private O2Api _o2Api = new O2Api();

        private IEnumerable<int> locationIds = new [] {127752};

        public IHttpActionResult GetStats(int ageGroup, int hour)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var lst = new List<object>();

            foreach (var locationId in locationIds)
            {
                var result = _o2Api.GetCountOfPeople(locationId, ageGroup, 1, hour);

                lst.Add(new { count = result, locationId });
            }

            return Ok(lst);
        }

        public IHttpActionResult GetCoords()
        {
            var t = new ZsjGeo().Load(locationIds);

            return Ok(t);
        }
    }
}
