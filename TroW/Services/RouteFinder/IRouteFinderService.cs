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
        RouteFinderOutputDto FindRoute(RouteFinderInputDto route);
    }
}
