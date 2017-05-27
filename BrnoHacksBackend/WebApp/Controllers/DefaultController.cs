using System.Net;
using System.Web.Http;
using WebApp.Integration;

namespace WebApp.Controllers
{
    public class DefaultController : ApiController
    {
        private O2Api _o2Api = new O2Api();

        public IHttpActionResult GetProduct(int id)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var result = _o2Api.GetCountOfPeople(127752, 2, 1, 10);

            return Ok(result);
        }

        public IHttpActionResult GetCoordinates()
        {
            var obj = new object[]
            {
                new { lat = 49.20, lng = 16.60},
                new { lat = 49.21, lng = 16.61},
                new { lat = 49.20, lng = 16.62},
                new { lat = 49.20, lng = 16.60}
            };

            return Ok(obj);
        }
    }
}
