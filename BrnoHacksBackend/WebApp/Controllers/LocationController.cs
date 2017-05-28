using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using WebApp.DAL;
using WebApp.Helpers;
using WebApp.Integration;

namespace WebApp.Controllers
{
    public class LocationController : ApiController
    {
        private int MinimalCount = 100;

        private GoVisiblyContext _ctx = new GoVisiblyContext();

        public IHttpActionResult GetStats(IEnumerable<int> locationIds, string authorizationToken = null, int? ageGroup = null, int? occurenceType = null, int? gender = null, int? hour = null)
        {
            CheckAuthorization(authorizationToken);

            var locationsIdsToCheck = _ctx.ZsjLocations.Where(x => x.IsEnabled)
                .Select(x => x.Id).ToList().AsEnumerable();

            if (locationIds != null)
            {
                locationsIdsToCheck = locationsIdsToCheck.Intersect(locationIds);
            }

            var dbData = _ctx.O2Datas.
                                    Where(x => x.AgeGroup == ageGroup &&
                                                 x.Gender == gender &&
                                                 x.OccurenceType == occurenceType.Value &&
                                                 x.Hour == hour.Value &&
                                                 locationsIdsToCheck.Contains(x.LocationId))
                                    .ToList();

            var counts = locationsIdsToCheck.Select(x =>
            {
                var dbEntry = dbData.FirstOrDefault(y => y.LocationId == x);
                int count = MinimalCount;
                if (dbEntry != null)
                {
                    count = dbEntry.Count;
                }

                return new {LocationId = x, Count = count};
            });

            return Ok(counts);
        }

        private Random _random = new Random();

        public IHttpActionResult GetStatsByLocation(int locationId, string authorizationToken = null)
        {
            CheckAuthorization(authorizationToken);

            var data = _ctx.O2Datas.Where(x => x.LocationId == locationId)
                .GroupBy(x => x.Hour)
                .ToList();

            return Ok(data.Select(x =>
            {
                return new
                {
                    hour = x.Key,
                    transit = new {
                        children = x.FirstOrDefault(y => y.AgeGroup == 1 && y.OccurenceType == 1)?.Count ?? MinimalCount,
                        youngsters = x.FirstOrDefault(y => y.AgeGroup == 2 && y.OccurenceType == 1)?.Count ?? MinimalCount,
                        middleage = x.FirstOrDefault(y => y.AgeGroup == 3 && y.OccurenceType == 1)?.Count ?? MinimalCount,
                        elders = x.FirstOrDefault(y => y.AgeGroup == 4 && y.OccurenceType == 1)?.Count ?? MinimalCount,
                        seniors = x.FirstOrDefault(y => y.AgeGroup == 5 && y.OccurenceType == 1)?.Count ?? MinimalCount,
                        male = x.FirstOrDefault(y => y.Gender == 1 && y.OccurenceType == 1)?.Count ?? MinimalCount,
                        female = x.FirstOrDefault(y => y.Gender == 2 && y.OccurenceType == 1)?.Count ?? MinimalCount,
                        all = x.FirstOrDefault(y => y.Gender == 1 && y.OccurenceType == 1)?.Count ?? MinimalCount + x.FirstOrDefault(y => y.Gender == 2 && y.OccurenceType == 1)?.Count ?? MinimalCount
                    },
                    visit = new
                    {
                        children = x.FirstOrDefault(y => y.AgeGroup == 1 && y.OccurenceType == 2)?.Count ?? MinimalCount,
                        youngsters = x.FirstOrDefault(y => y.AgeGroup == 2 && y.OccurenceType == 2)?.Count ?? MinimalCount,
                        middleage = x.FirstOrDefault(y => y.AgeGroup == 3 && y.OccurenceType == 2)?.Count ?? MinimalCount,
                        elders = x.FirstOrDefault(y => y.AgeGroup == 4 && y.OccurenceType == 2)?.Count ?? MinimalCount,
                        seniors = x.FirstOrDefault(y => y.AgeGroup == 5 && y.OccurenceType == 2)?.Count ?? MinimalCount,
                        male = x.FirstOrDefault(y => y.Gender == 1 && y.OccurenceType == 2)?.Count ?? MinimalCount,
                        female = x.FirstOrDefault(y => y.Gender == 2 && y.OccurenceType == 2)?.Count ?? MinimalCount,
                        all = x.FirstOrDefault(y => y.Gender == 1 && y.OccurenceType == 2)?.Count ?? MinimalCount + x.FirstOrDefault(y => y.Gender == 2 && y.OccurenceType == 2)?.Count ?? MinimalCount
                    },
                    transport = _random.Next(10, 20)
                };
            }));
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
            var dbLocations = _ctx.ZsjLocations.Where(x => x.IsEnabled);

            var locations = dbLocations.ToList().Select(x => new
            {
                LocationId = x.Id,
                Coordinates = ParseCoordinates(x.Coordinates),
                x.Name
            });

            return Ok(locations);
        }

        private IEnumerable<ZsjGeo.GeoPoint> ParseCoordinates(string coordinates)
        {
            List<ZsjGeo.GeoPoint> points = new List<ZsjGeo.GeoPoint>();

            foreach (var coords in coordinates.Split(new [] {"]["}, StringSplitOptions.RemoveEmptyEntries))
            {
                var vals = coords.Split(';');

                points.Add(new ZsjGeo.GeoPoint
                {
                    lat = float.Parse(vals[0].Trim('[', ']', ';')),
                    lng = float.Parse(vals[1].Trim('[', ']', ';'))
                });
            }

            return points;
        }
    }
}
