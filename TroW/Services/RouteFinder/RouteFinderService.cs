using System.Collections.Generic;
using System.Linq;
using DatabaseContext.DatabaseAcces;
using DatabaseContext.Domain;
using Services.RouteFinder.Dto;

namespace Services.RouteFinder
{
    public class RouteFinderService:IRouteFinderService
    {
        private readonly IRepository<TrainHistoryData> _trainHistoryDataRepository;
        private readonly IRepository<Tren> _trenRepository;
        private readonly IRepository<Trasa> _trasaRepository;
        private readonly IRepository<Statie> _statieRepository;
        public RouteFinderService(IRepository<TrainHistoryData> trainHistoryDataRepository, IRepository<Tren> trenRepository, IRepository<Trasa> trasaRepository, IRepository<Statie> statieRepository)
        {
            _trainHistoryDataRepository = trainHistoryDataRepository;
            _trenRepository = trenRepository;
            _trasaRepository = trasaRepository;
            _statieRepository = statieRepository;
        }

        public AllRoutesResultsDto FindRoute(RouteFinderInputDto route)
        {

            var point1 = new PointDto { OraAjungere = 1230,CodStatie = 111, StatieNume = "Tileagd", Position = "47.0538895, 22.209420200000068" };
            var point2 = new PointDto { OraAjungere = 3330,CodStatie = 112 ,StatieNume = "Oradea Est", Position = "47.0432383, 21.962399499999947" };
            var point3 = new PointDto { OraAjungere = 1111,CodStatie = 113 ,StatieNume = "Piata Old", Position = "44.3666767, 24.2540166" };
            var point4 = new PointDto { OraAjungere = 2222,CodStatie = 114, StatieNume = "Slatina", Position = "44.4283955, 24.38860390000002" };
            var point5 = new PointDto { OraAjungere = 3344,CodStatie = 115, StatieNume = "Costesti", Position = "44.6742056, 24.857733999999937" };
            var routeOutput = new AllRoutesResultsDto();
            routeOutput.Routes = new List<RouteDto>();
            routeOutput.Routes.Add(new RouteDto {Points = new List<PointDto> {point1, point2}});
            routeOutput.Routes.Add(new RouteDto { Points = new List<PointDto> { point4, point2, point1, point2 } });
            return routeOutput;
        }

        public List<StatieDto> GetStationThatHaveNameWithTerm(string term)
        {
            var stations =
              (from station in _statieRepository.GetQuery(x => (x.NumeStatie).ToLower().Contains(term.ToLower()))
               select new StatieDto{CodStatie = station.CodStatie, NumeStatie = station.NumeStatie}).ToList();
            return stations;
        }
    }
}
