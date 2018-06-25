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

        public IReadOnlyCollection<Location> GetLocationsByCity(string city)
        {
            if (!_db.Locations.Any() || string.IsNullOrEmpty(city))
            {
                return null;
            }

            var first = 0;
            int last = _db.SortedLocationIndexes.Count;

            var foundLocations = new List<Location>();
            while (first <= last)
            {
                int mid = first + (last - first) / 2;
                var midIdx = _db.SortedLocationIndexes.ElementAt(mid);
                var midLoc = _db.Locations.ElementAt((int)midIdx);

                if (string.Equals(city, midLoc.City, StringComparison.OrdinalIgnoreCase))
                {
                    foundLocations.Add(midLoc);
                    continue;
                }

                var comparisonResult = string.Compare(city, midLoc.City, StringComparison.OrdinalIgnoreCase);
                if (comparisonResult < 0)
                {
                    last = mid - 1;
                }
                else
                {
                    first = mid + 1;
                }
            }

            return foundLocations;
        }
    }
}