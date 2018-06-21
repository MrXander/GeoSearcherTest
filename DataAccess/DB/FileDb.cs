using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DataAccess
{
    internal class FileDb
    {
        private string _filePath;

        public FileDb(string filePath)
        {
            if(!File.Exists(filePath))
                throw new FileNotFoundException(nameof(filePath));

            _filePath = filePath;
        }

        public void Load()
        {
            var bytes = File.ReadAllBytes(geoBasePath);
            using (var geoBaseReader = new FileDbReader(bytes))
            {
                Header header = geoBaseReader.ReadHeader();
                Range[] ranges = geoBaseReader.ReadIpRanges(header.Records);
                Location[] locations = geoBaseReader.ReadIpLocations(header.Records);
                uint[] indexes = geoBaseReader.ReadIndexes(header.Records);
                //---
                stopwatch.Stop();
                //---
                dataBase = new DataBase(header, ranges, locations, indexes, stopwatch.ElapsedMilliseconds);
            }

        }
    }

    internal class FileDbReader : IDisposable
    {
        const int HeaderSize = 60;

        const int RangesSize = 20;

        const int LocationsSize = 96;

        const int IndexesSize = 4;

        public FileDbReader(byte[] bytes)
        {
            _binaryReader = new BinaryReader(new MemoryStream(bytes));
        }

        private readonly BinaryReader _binaryReader;
       

        public Header ReadHeader()
        {
            ThrowIfCantReadStream();
            //---
            _binaryReader.BaseStream.Seek(0, SeekOrigin.Begin);
            //---
            Header header;
            //---
            header.Version = _binaryReader.ReadInt32();
            header.Name = _binaryReader.ReadChars(32);
            header.Date = _binaryReader.ReadUInt64();
            header.Records = _binaryReader.ReadInt32();
            header.OffsetRanges = _binaryReader.ReadUInt32();
            header.OffsetCities = _binaryReader.ReadUInt32();
            header.OffsetLocations = _binaryReader.ReadUInt32();
            //---
            return header;
        }

        public Range[] ReadIpRanges(int records)
        {
            ThrowIfCantReadStream();
            //---
            if (_binaryReader.BaseStream.Position != HeaderSize)
            {
                _binaryReader.BaseStream.Seek(HeaderSize, SeekOrigin.Begin);
            }
            //---
            Range[] ranges = new Range[records];
            //---
            for (int i = 0; i < records; i++)
            {
                ranges[i].IpFrom = _binaryReader.ReadUInt64();
                ranges[i].IpTo = _binaryReader.ReadUInt64();
                ranges[i].Index = _binaryReader.ReadUInt32();
            }
            //---
            return ranges;
        }

        public Location[] ReadIpLocations(int records)
        {
            ThrowIfCantReadStream();
            //---
            int offset = RangesSize * records + HeaderSize;
            if (_binaryReader.BaseStream.Position != offset)
            {
                _binaryReader.BaseStream.Seek(offset, SeekOrigin.Begin);
            }
            Location[] locations = new Location[records];
            //---
            for (int i = 0; i < records; i++)
            {
                locations[i].Country = _binaryReader.ReadChars(8);
                locations[i].Region = _binaryReader.ReadChars(12);
                locations[i].Postal = _binaryReader.ReadChars(12);
                locations[i].City = _binaryReader.ReadChars(24);
                locations[i].Organization = _binaryReader.ReadChars(32);
                locations[i].Latitude = _binaryReader.ReadSingle();
                locations[i].Longitude = _binaryReader.ReadSingle();
            }
            //---
            return locations;
        }

        public uint[] ReadIndexes(int records)
        {
            ThrowIfCantReadStream();
            //---
            int offset = LocationsSize * records + RangesSize * records + HeaderSize;
            if (_binaryReader.BaseStream.Position != offset)
            {
                _binaryReader.BaseStream.Seek(offset, SeekOrigin.Begin);
            }
            //---
            uint[] indexes = new uint[records];
            //---
            for (int i = 0; i < records; i++)
            {
                indexes[i] = _binaryReader.ReadUInt32();
            }
            //---
            return indexes;
        }

        public void Dispose()
        {
            _binaryReader.Dispose();
        }

        private void ThrowIfCantReadStream()
        {
            if (!_binaryReader.BaseStream.CanRead)
            {
                throw new Exception("Can't read database");
            }
        }
    }
}
