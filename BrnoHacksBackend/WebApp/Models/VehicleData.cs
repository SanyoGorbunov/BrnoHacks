using System;

namespace WebApp.Models
{
    public class VehicleData
    {
        public int Id { get; set; }

        public int VehicleId { get; set; }

        public double Lat { get; set; }

        public double Lng { get; set; }

        public int Route { get; set; }

        public string Course { get; set; }

        public int Bearing { get; set; }

        public string HeadSign { get; set; }

        public int LocationId { get; set; }

        public DateTime AsAt { get; set; }

        public DateTime DateCreated { get; set; }
    }
}