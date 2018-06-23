using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.DB;
using DataAccess.Models;

namespace DataAccess
{
    public class GeoRepository : IGeoRepository
    {
        private readonly GeoDb _db;

        public GeoRepository(FileDbManager dbManager)
        {
            _db = dbManager.Load();
        }

        public Location GetLocationsByIP(ulong ip)
        {
            if (!_db.Ranges.Any())
            {
                return null;
            }

            var first = 0;
            int last = _db.Ranges.Count;

            while (first <= last)
            {
                int mid = first + (last - first) / 2;

                IPRange midRange = _db.Ranges.ElementAt(mid);

                if (ip >= midRange.IpFrom && ip <= midRange.IpTo)
                {
                    //TODO bad cast ?
                    return _db.Locations.ElementAt((int) midRange.Index);
                }

                if (ip < midRange.IpFrom)
                {
                    last = mid - 1;
                }
                else
                {
                    first = mid + 1;
                }
            }

            return null;
        }

        public IReadOnlyCollection<Location> GetLocationsByCity(string city) => throw new NotImplementedException();
    }
}