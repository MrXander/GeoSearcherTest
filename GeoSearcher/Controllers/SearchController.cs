using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using BusinessLogic;
using BusinessLogic.Models;
using Microsoft.AspNetCore.Mvc;

namespace GeoSearcher.Controllers
{
    [Route("/")]
    public class SearchController : Controller
    {
        private static readonly string[] Summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

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
                       : new [] { location };
        }

        [HttpGet("/ip/locations")]
        public IEnumerable<Models.Location> City(string city)
        {
            var rng = new Random();
            return Enumerable.Range(1,
                                    5)
                             .Select(index => new Models.Location
                                         ());
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