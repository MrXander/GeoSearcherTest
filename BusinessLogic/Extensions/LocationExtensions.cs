#region

using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Models;

#endregion

namespace BusinessLogic.Extensions
{
    internal static class LocationExtensions
    {
        public static Location ToBusiness(this DataAccess.Models.Location location)
        {
            if (location == null)
            {
                return null;
            }

            return new Location(location.Country,
                                location.Region,
                                location.Postal,
                                location.City,
                                location.Organization,
                                location.Latitude,
                                location.Longitude);
        }

        public static IReadOnlyCollection<Location> ToBusiness(this IEnumerable<DataAccess.Models.Location> locations)
        {
            return locations?.Select(loc => loc.ToBusiness())
                             .ToArray();
        }
    }
}