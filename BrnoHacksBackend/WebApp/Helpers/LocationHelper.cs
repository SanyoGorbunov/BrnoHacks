using System.Collections.Generic;

namespace WebApp.Helpers
{
    public class LocationHelper
    {
        private static IEnumerable<int> _locationIds = new[]
        {
            19, 20
        };

        public static IEnumerable<int> LocationIds
        {
            get { return _locationIds; }
        }
    }
}