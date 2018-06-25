using System;
using System.Collections.Generic;
using System.Net;
using BusinessLogic;
using BusinessLogic.Models;
using Microsoft.AspNetCore.Mvc;

namespace GeoSearcher.Controllers
{
    [Route("/")]
    public class SearchController : Controller
    {
        private readonly IGeoSearcher _geoSearcher;

        public SearchController(IGeoSearcher geoSearcher)
        {
            _geoSearcher = geoSearcher;
        }

        [HttpGet("/ip/location")]
        // ReSharper disable once InconsistentNaming
        public IEnumerable<Location> IP(string ip)
        {
            if (!TryParseIP(ip,
                            out ulong parsedIp))
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return null;
            }

            var location = _geoSearcher.GetLocationByIP(parsedIp);
            return location == null
                       ? new Location[0]
                       : new[] { location };
        }

        [HttpGet("/city/locations")]
        public IReadOnlyCollection<Location> City(string city)
        {
            if (string.IsNullOrEmpty(city))
                return new Location[0];

            return _geoSearcher.GetLocationsByCity(city);
        }

        private bool TryParseIP(string ip,
                                out ulong parsedIp)
        {
            parsedIp = 0;
            if (!IPAddress.TryParse(ip,
                                    out IPAddress ipAddress))
            {
                return false;
            }

            parsedIp = BitConverter.ToUInt32(ipAddress.GetAddressBytes(),
                                             0);

            return true;
        }
    }
}