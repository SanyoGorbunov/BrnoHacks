using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using WebApp.Helpers;

namespace WebApp.Controllers
{
    public class DefaultController : ApiController
    {
        public IHttpActionResult GetProduct(int id)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            string result;

            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    RequestUri =
                        new Uri(
                            ConfigurationHelper.O2BasePath + "/age/127752?ageGroup=2&occurenceType=1&hour=10"),
                    Method = HttpMethod.Get
                };

                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                request.Headers.Add("apikey", ConfigurationHelper.ApiKey);

                result = client.SendAsync(request).Result.Content.ReadAsStringAsync().Result;
            }
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
