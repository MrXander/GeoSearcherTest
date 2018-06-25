﻿#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DataAccess.Models;

#endregion

namespace DataAccess.DB
{
    internal class FileDbReader : IDisposable
    {
        private readonly BinaryReader _binaryReader;

        public FileDbReader(byte[] bytes)
        {
            _binaryReader = new BinaryReader(new MemoryStream(bytes));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Header ReadHeader()
        {
            _binaryReader.BaseStream.Seek(0,
                                          SeekOrigin.Begin);
            return new Header(
                              version: _binaryReader.ReadInt32(),
                              name: new string(_binaryReader.ReadChars(32)).TrimEnd('\0'),
                              date: _binaryReader.ReadUInt64(),
                              records: _binaryReader.ReadInt32(),
                              offsetRanges: _binaryReader.ReadUInt32(),
                              offsetCities: _binaryReader.ReadUInt32(),
                              offsetLocations: _binaryReader.ReadUInt32());
        }

        public IReadOnlyCollection<IPRange> ReadIpRanges(int records)
        {
            var ranges = new IPRange[records];
            for (var i = 0; i < records; i++)
            {
                ranges[i] = new IPRange(
                                        _binaryReader.ReadUInt32(),
                                        _binaryReader.ReadUInt32(),
                                        _binaryReader.ReadUInt32());
            }

            return ranges;
        }

        public IReadOnlyCollection<Location> ReadIpLocations(int records)
        {
            var locations = new Location[records];
            for (var i = 0; i < records; i++)
            {
                locations[i] = new Location(
                                            country: new string(_binaryReader.ReadChars(8)).TrimEnd('\0'),
                                            region: new string(_binaryReader.ReadChars(12)).TrimEnd('\0'),
                                            postal: new string(_binaryReader.ReadChars(12)).TrimEnd('\0'),
                                            city: new string(_binaryReader.ReadChars(24)).TrimEnd('\0'),
                                            organization: new string(_binaryReader.ReadChars(32)).TrimEnd('\0'),
                                            latitude: _binaryReader.ReadSingle(),
                                            longitude: _binaryReader.ReadSingle());
            }

            return locations;
        }

        public uint[] ReadSortedLocationIndexes(uint locationsOffset, int records)
        {
            const int locationSizeBytes = 96;

            var idx = new uint[records];
            for (int i = 0; i < records; i++)
            {
                idx[i] = _binaryReader.ReadUInt32();
            }

            idx = idx.OrderBy(x => x)
                     .ToArray();
;            var indexes = new uint[records];
            for (var i = 0; i < records; i++)
            {
                //var offsetFromLocations = _binaryReader.ReadUInt32();
                var offsetFromLocations = idx[i];
                var index = (locationsOffset > offsetFromLocations
                                 ? offsetFromLocations
                                 : offsetFromLocations - locationsOffset) / locationSizeBytes;
                var a = locationsOffset > offsetFromLocations
                            ? offsetFromLocations
                            : offsetFromLocations - locationsOffset;
                if (a % locationSizeBytes > 0)
                    throw new Exception("ooops");
                indexes[i] = index;
            }

            return indexes;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_binaryReader != null)
                {
                    _binaryReader.Dispose();
                }
            }
        }
    }
}