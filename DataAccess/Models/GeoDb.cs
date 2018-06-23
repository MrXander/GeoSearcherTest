#region

using System.Collections.Generic;

#endregion

namespace DataAccess.Models
{
    public class GeoDb
    {
        public GeoDb(Header header,
                     IReadOnlyCollection<IPRange> ranges,
                     IReadOnlyCollection<Location> locations,
                     uint[] indexes)
        {
            Header = header;
            Ranges = ranges;
            Locations = locations;
            Indexes = indexes;
        }

        public Header Header { get; }
        public IReadOnlyCollection<IPRange> Ranges { get; }
        public IReadOnlyCollection<Location> Locations { get; }
        public uint[] Indexes { get; }
    }
}