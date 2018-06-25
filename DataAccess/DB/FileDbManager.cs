using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DataAccess.Models;

namespace DataAccess.DB
{
    public class FileDbManager
    {
        private readonly string _filePath;
        private GeoDb _db;

        public FileDbManager(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(nameof(filePath));
            }

            _filePath = filePath;
        }

        public GeoDb Load()
        {
            if (_db == null)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                byte[] bytes = File.ReadAllBytes(_filePath);
                stopwatch.Stop();
                Debug.WriteLine($"Db loaded in {stopwatch.ElapsedMilliseconds} ms.");

                using (var dbReader = new FileDbReader(bytes))
                {
                    Header header = dbReader.ReadHeader();
                    IReadOnlyCollection<IPRange> ranges = dbReader.ReadIpRanges(header.Records);
                    IReadOnlyCollection<Location> locations = dbReader.ReadIpLocations(header.Records);
                    uint[] indexes = dbReader.ReadSortedLocationIndexes(header.OffsetLocations, header.Records);

                    ////indexes is not sorted by city. Uncomment to see result in Output
                    //var cityNames = new List<string>(indexes.Length);
                    //for (int i = 0; i < indexes.Length; i++)
                    //{
                    //    cityNames.Add(locations.ElementAt((int)indexes[i]).City);
                    //}
                    //Debug.WriteLine(string.Join("\r\n", cityNames));

                    _db = new GeoDb(header,
                                    ranges,
                                    locations,
                                    indexes);
                }
            }

            return _db;
        }
    }
}