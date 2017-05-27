using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

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
                                    geoPoints.Add(new GeoPoint { lat = lat.Value, lng = val });
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