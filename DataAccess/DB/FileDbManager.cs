using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using DataAccess.Models;

namespace DataAccess.DB
{
    public class FileDbManager
    {
        private readonly string _filePath;
        private GeoDb _db;

        public FileDbManager(string filePath)
        {
            var currenctDir = Directory.GetCurrentDirectory();
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
                using (var dbReader = new FileDbReader(bytes))
                {
                    Header header = dbReader.ReadHeader();
                    //Debug.WriteLine("aaaaa");
                    IReadOnlyCollection<IPRange> ranges = dbReader.ReadIpRanges(header.Records);
                    IReadOnlyCollection<Location> locations = dbReader.ReadIpLocations(header.Records);
                    uint[] indexes = dbReader.ReadIndexes(header.Records);

                    stopwatch.Stop();

                    Debug.WriteLine($"Db loaded in {stopwatch.ElapsedMilliseconds} ms.");

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