using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using WebApp.DAL;
using WebApp.Models;

namespace WebApp.Integration
{
    public class ZsjGeo
    {
        public class ZsjGeoPoint
        {
            public int LocationId { get; set; }

            public IEnumerable<GeoPoint> Coordinates { get; set; }
        }

        public class GeoPoint
        {
            public float lat { get; set; }

            public float lng { get; set; }
        }

        public void StoreLocalPoints(string csvPath, string jsonPath)
        {
            var locations = new List<ZsjLocation>();

            bool isHeader = true;

            foreach (var csvLine in File.ReadAllLines(csvPath))
            {
                if (isHeader)
                {
                    isHeader = false;
                    continue;
                }

                var tokens = csvLine.Split(new char[] {';'}, StringSplitOptions.None);
                
                locations.Add(new ZsjLocation
                {
                    Id = int.Parse(tokens[0]),
                    Name = tokens[1]
                });
            }

            var locationIds = locations.Select(x => x.Id);

            using (var stream = File.OpenRead(jsonPath))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    using (var jsonReader = new JsonTextReader(streamReader))
                    {
                        bool isZsjCode = false;
                        int? currentZsj = null;
                        bool startCoords = false;
                        int bracketsOpen = 0;
                        List<GeoPoint> geoPoints = null;
                        float? lat = null;

                        while (jsonReader.Read())
                        {
                            //evaluate the current node and whether it's the name you want
                            if (jsonReader.TokenType == JsonToken.PropertyName && string.Equals(jsonReader.Value as string, "zsj_code"))
                            {
                                isZsjCode = true;
                            }
                            else if (jsonReader.TokenType == JsonToken.Integer && isZsjCode)
                            {
                                var zsjId = int.Parse(jsonReader.Value.ToString());

                                if (locationIds.Contains(zsjId))
                                {
                                    currentZsj = zsjId;
                                }
                                isZsjCode = false;
                            }
                            else if (jsonReader.TokenType == JsonToken.PropertyName && currentZsj.HasValue &&
                                string.Equals(jsonReader.Value as string, "coordinates"))
                            {
                                startCoords = true;

                                geoPoints = new List<GeoPoint>();
                            }
                            else if (jsonReader.TokenType == JsonToken.StartArray && startCoords)
                            {
                                bracketsOpen++;
                            }
                            else if (jsonReader.TokenType == JsonToken.EndArray && startCoords)
                            {
                                bracketsOpen--;
                                if (bracketsOpen == 0)
                                {
                                    startCoords = false;

                                    locations.Single(x => x.Id == currentZsj.Value).Coordinates = GetGeoPointsString(geoPoints);

                                    currentZsj = null;
                                }
                            }
                            else if (jsonReader.TokenType == JsonToken.Float && startCoords)
                            {
                                var val = float.Parse(jsonReader.Value.ToString());

                                if (lat.HasValue)
                                {
                                    geoPoints.Add(new GeoPoint { lat = val, lng = lat.Value });

                                    lat = null;
                                }
                                else
                                {
                                    lat = val;
                                }
                            }
                        }
                    }
                }
            }

            using (GoVisiblyContext ctx = new GoVisiblyContext())
            {
                ctx.ZsjLocations.AddRange(locations);
                ctx.SaveChanges();
            }
        }

        private string GetGeoPointsString(IEnumerable<GeoPoint> points)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var point in points)
            {
                sb.Append($"[{point.lat};{point.lng}]");
            }

            return sb.ToString();
        }

        public IEnumerable<ZsjGeoPoint> Load(string path, IEnumerable<int> locationIds)
        {
            var zsjGeoPoints = new List<ZsjGeoPoint>();

            using (var stream = File.OpenRead(path))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    using (var jsonReader = new JsonTextReader(streamReader))
                    {
                        bool isZsjCode = false;
                        int? currentZsj = null;
                        bool startCoords = false;
                        int bracketsOpen = 0;
                        List<GeoPoint> geoPoints = null;
                        float? lat = null;

                        while (jsonReader.Read())
                        {
                            //evaluate the current node and whether it's the name you want
                            if (jsonReader.TokenType == JsonToken.PropertyName && string.Equals(jsonReader.Value as string, "zsj_code"))
                            {
                                isZsjCode = true;
                            }
                            else if (jsonReader.TokenType == JsonToken.Integer && isZsjCode)
                            {
                                var zsjId = int.Parse(jsonReader.Value.ToString());

                                if (locationIds.Contains(zsjId))
                                {
                                    currentZsj = zsjId;
                                }
                                isZsjCode = false;
                            }
                            else if (jsonReader.TokenType == JsonToken.PropertyName && currentZsj.HasValue &&
                                string.Equals(jsonReader.Value as string, "coordinates"))
                            {
                                startCoords = true;

                                geoPoints = new List<GeoPoint>();
                            }
                            else if (jsonReader.TokenType == JsonToken.StartArray && startCoords)
                            {
                                bracketsOpen++;
                            }
                            else if (jsonReader.TokenType == JsonToken.EndArray && startCoords)
                            {
                                bracketsOpen--;
                                if (bracketsOpen == 0)
                                {
                                    startCoords = false;

                                    zsjGeoPoints.Add(new ZsjGeoPoint
                                    {
                                        LocationId = currentZsj.Value,
                                        Coordinates = geoPoints
                                    });

                                    currentZsj = null;
                                }
                            }
                            else if (jsonReader.TokenType == JsonToken.Float && startCoords)
                            {
                                var val = float.Parse(jsonReader.Value.ToString());

                                if (lat.HasValue)
                                {
                                    geoPoints.Add(new GeoPoint { lat = val, lng = lat.Value });

                                    lat = null;
                                }
                                else
                                {
                                    lat = val;
                                }
                            }
                        }
                    }
                }
            }

            return zsjGeoPoints;
        }
    }
}