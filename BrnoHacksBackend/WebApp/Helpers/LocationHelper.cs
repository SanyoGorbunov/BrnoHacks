using System.Collections.Generic;

namespace WebApp.Helpers
{
    public class LocationHelper
    {
        private static IEnumerable<int> _locationIds = new[]
        {
            10006, 10031, 10014, 10022
        };

        public static IEnumerable<int> LocationIds
        {
            get { return _locationIds; }
        }
    }
}