using System;
using System.Collections.Generic;
using System.Text;
using DataAccess.Models;

namespace DataAccess
{
    public class GeoRepository: IGeoRepository
    {
        public GeoRepository(string dbPath)
        {
            
        }
        
        public IReadOnlyCollection<Location> GetLocationsByIP(string ip)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<Location> GetLocationsByCity(string city)
        {
            throw new NotImplementedException();
        }
    }
}
