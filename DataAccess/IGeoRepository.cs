#region

using System.Collections.Generic;
using DataAccess.Models;

#endregion

namespace DataAccess
{
    public interface IGeoRepository
    {
        IReadOnlyCollection<Location> GetLocationsByIP(string ip);
        IReadOnlyCollection<Location> GetLocationsByCity(string city);
    }
}