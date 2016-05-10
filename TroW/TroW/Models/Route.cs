using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TroW.Models
{
    public class AllRoutesResults
    {
        public List<Route> Routes { get; set; }
    }
    public class Route
    {
        public List<Point> Points { get; set; }
    }

    public class Point
    {
        public string StatieNume { get; set; }
        public int OraPlecare { get; set; }
        public int OraAjungere { get; set; }
    }
}