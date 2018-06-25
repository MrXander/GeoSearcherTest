#region

using System.Collections.Generic;
using BusinessLogic.Extensions;
using BusinessLogic.Models;
using DataAccess;

#endregion

namespace BusinessLogic
{
    public class GeoSearcher : IGeoSearcher
    {
        private readonly IGeoRepository _repository;

        public GeoSearcher(IGeoRepository repository)
        {
            _repository = repository;
        }

        public Location GetLocationByIP(ulong ip)
        {
            DataAccess.Models.Location location = _repository.GetLocationsByIP(ip);
            return location?.ToBusiness();
        }

        public IReadOnlyCollection<Location> GetLocationsByCity(string city)
        {
            var locations = _repository.GetLocationsByCity(city);
            return locations?.ToBusiness();
        }
    }
}