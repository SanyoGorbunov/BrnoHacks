using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using WebApp.DAL;
using WebApp.Helpers;
using WebApp.Integration;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class UpdatesController: ApiController
    {
        private GoVisiblyContext _ctx = new GoVisiblyContext();

        private string _basePath = ConfigurationHelper.O2BasePath;

        private string _apiKey = ConfigurationHelper.ApiKey;

        public IHttpActionResult GetO2(int locationId, int hour, int occurenceType, int? ageGroup = null,
            int? gender = null)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            using (var client = new HttpClient())
            {
                string uri = CreateUri(locationId, hour, occurenceType, ageGroup, gender);

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(uri),
                    Method = HttpMethod.Get
                };

                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                request.Headers.Add("apikey", _apiKey);

                var response = client.SendAsync(request).Result;

                int count = -1;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var json = response.Content.ReadAsStringAsync().Result;

                    var jObj = JObject.Parse(json);

                    count = jObj["count"].Value<int>();
                }

                var data = new OTwoData
                {
                    AgeGroup = ageGroup,
                    Gender = gender,
                    Hour = hour,
                    LocationId = locationId,
                    OccurenceType = occurenceType,
                    DateCreated = DateTime.UtcNow,
                    Url = uri,
                    Count = count
                };

                _ctx.O2Datas.Add(data);

                _ctx.SaveChanges();
            }

            return Ok();
        }

        private string CreateUri(int locationId, int hour, int occurenceType, int? ageGroup, int? gender)
        {
            string ageGroupQuery = ageGroup.HasValue ? "ageGroup=" + ageGroup.Value + "&" : string.Empty;
            string genderQuery = gender.HasValue ? "gender=" + gender.Value + "&" : string.Empty;
            string methodName = ageGroup.HasValue ? "age" : "gender";

            return $"{_basePath}/{methodName}/{locationId}?{ageGroupQuery}{genderQuery}occurenceType={occurenceType}&hour={hour}";
        }

        public IHttpActionResult GetZsj()
        {
            new ZsjGeo().StoreLocalPoints(ConfigurationHelper.CsvGeoPath, ConfigurationHelper.ZsjGeoPath);

            return Ok();
        }

        public IHttpActionResult GetO2FileAge()
        {
            var locationIds = new HashSet<int>(_ctx.ZsjLocations.Where(x => x.IsEnabled).Select(x => x.Id).ToList());

            var lines = File.ReadAllLines(ConfigurationHelper.O2SampleAge);

            var o2data = new List<OTwoData>();

            bool isFirstLine = true;
            foreach (var line in lines)
            {
                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue;
                }

                var tokens = line.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);

                if (locationIds.Contains(int.Parse(tokens[2])))
                {
                    int h = int.Parse(tokens[1]);
                    int o = int.Parse(tokens[3]);
                    int a = int.Parse(tokens[4]);
                    int cnt = int.Parse(tokens[5]);

                    o2data.Add(new OTwoData
                    {
                        AgeGroup = a,
                        Count = cnt,
                        DateCreated = DateTime.UtcNow,
                        Hour = h,
                        Gender = null,
                        OccurenceType = o,
                        LocationId = int.Parse(tokens[2])
                    });
                }
            }

            _ctx.O2Datas.AddRange(o2data);
            _ctx.SaveChanges();

            return Ok();
        }

        public IHttpActionResult GetO2FileGender()
        {
            var locationIds = new HashSet<int>(_ctx.ZsjLocations.Where(x => x.IsEnabled).Select(x => x.Id).ToList());

            var lines = File.ReadAllLines(ConfigurationHelper.O2SampleGender);

            var o2data = new List<OTwoData>();

            bool isFirstLine = true;
            foreach (var line in lines)
            {
                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue;
                }

                var tokens = line.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                if (locationIds.Contains(int.Parse(tokens[2])))
                {
                    int h = int.Parse(tokens[1]);
                    int o = int.Parse(tokens[3]);
                    int g = int.Parse(tokens[4]);
                    int cnt = int.Parse(tokens[5]);

                    o2data.Add(new OTwoData
                    {
                        Gender = g,
                        Count = cnt,
                        DateCreated = DateTime.UtcNow,
                        Hour = h,
                        AgeGroup = null,
                        OccurenceType = o,
                        LocationId = int.Parse(tokens[2])
                    });
                }
            }

            _ctx.O2Datas.AddRange(o2data);
            _ctx.SaveChanges();

            return Ok();
        }
    }
}