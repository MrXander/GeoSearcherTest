using System.Collections.Generic;
using BusinessLogic.Models;

namespace BusinessLogic
{
    public interface IGeoSearcher
    {
        // ReSharper disable once InconsistentNaming
        Location GetLocationByIP(ulong ip);
        IReadOnlyCollection<Location> GetLocationsByCity(string city);
    }
}