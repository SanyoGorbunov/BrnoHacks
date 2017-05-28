using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class OTwoData
    {
        public int Id { get; set; }

        public int LocationId { get; set; }

        public int Hour { get; set; }

        public int OccurenceType { get; set; }

        public int? Gender { get; set; }

        public int? AgeGroup { get; set; }

        public int Count { get; set; }

        public string Url { get; set; }

        public DateTime DateCreated { get; set; }
    }
}