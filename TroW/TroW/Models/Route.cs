using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Services.RouteFinder.Dto;

namespace TroW.Models
{
    public class AllRoutesResults
    {
        public List<Route> Routes { get; set; }
        public String StopOverStations { get; set; }
        public RouteFinderInputDto Input { get; set; }
        
        
    }
    public class Route
    {
        public List<Point> Points { get; set; }
        public string TotalTime { get; set; }
    }

    public class Point
    {
        public string NumeStatie { get; set; }
        public int CodStatie { get; set; }
        public string OraAjungere { get; set; }
        public string LatLong { get; set; }
        public string Description { get; set; }
    }
}