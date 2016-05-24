using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.RouteFinder.Dto
{
    public class AllRoutesResultsDto //asta este clasa returnata
    {
        public List<RouteDto> Routes { get; set; }
    }

    public class RouteDto
    {
        public List<PointDto> Points { get; set; }
    }

    public class PointDto
    {
        public string StatieNume { get; set; }
        public int CodStatie { get; set; }
        public int OraPlecare { get; set; }
        public int OraAjungere { get; set; }
        public string Position { get; set; }
    }
}
