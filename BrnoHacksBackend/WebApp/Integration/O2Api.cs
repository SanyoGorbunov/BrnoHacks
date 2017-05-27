using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using WebApp.Helpers;

namespace WebApp.Integration
{
    public class O2Api
    {
        private string _basePath = ConfigurationHelper.O2BasePath;

        private string _apiKey = ConfigurationHelper.ApiKey;

        public int GetCountOfPeople(int locationId, int ageGroup, int occurenceType, int hour)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    RequestUri =
                        new Uri(
                            $"{_basePath}/age/{locationId}?ageGroup={ageGroup}&occurenceType={occurenceType}&hour={hour}"),
                    Method = HttpMethod.Get
                };

                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                request.Headers.Add("apikey", _apiKey);

                var response = client.SendAsync(request).Result;
                
                var json = response.Content.ReadAsStringAsync().Result;

                var jObj = JObject.Parse(json);

                return jObj["count"].Value<int>();
            }
        }
    }
}