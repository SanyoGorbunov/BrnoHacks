using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using WebApp.DAL;
using WebApp.Helpers;
using WebApp.Integration;

namespace WebApp.Controllers
{
    public class LocationController : ApiController
    {
        private O2Api _o2Api = new O2Api();

        public IHttpActionResult GetStats(IEnumerable<int> locationIds, string authorizationToken = null, int? ageGroup = null, int? occurenceType = null, int? gender = null, int? hour = null)
        {
            CheckAuthorization(authorizationToken);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var lst = new List<object>();

            var locationsIdsToCheck = LocationHelper.LocationIds;

            if (locationIds != null)
            {
                locationsIdsToCheck = locationsIdsToCheck.Intersect(locationIds);
            }

            foreach (var locationId in locationsIdsToCheck)
            {
                var result = _o2Api.GetCountOfPeople(locationId, ageGroup.Value, occurenceType.Value, hour.Value);

                lst.Add(new { count = result, locationId });
            }

            return Ok(lst);
        }

        private void CheckAuthorization(string authorizationToken)
        {
            if (!string.Equals(authorizationToken, ConfigurationHelper.AuthorizationToken, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
        }

        public IHttpActionResult GetCoords()
        {
            var zsjGeoPath = HostingEnvironment.MapPath(ConfigurationHelper.ZsjGeoPath);

            var t = new ZsjGeo().Load(zsjGeoPath, LocationHelper.LocationIds);

            return Ok(t);
        }

        public IHttpActionResult GetTest()
        {
            GoVisiblyContext ctx = new GoVisiblyContext();

            return Ok(ctx.ZsjLocations);
        }
    }
}
