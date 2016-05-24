using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.RouteFinder.Dto;

namespace Services.RouteFinder
{
    public interface IRouteFinderService
    {
        AllRoutesResultsDto FindRoute(RouteFinderInputDto route);
        List<StatieDto> GetStationThatHaveNameWithTerm(string term);
    }
}
