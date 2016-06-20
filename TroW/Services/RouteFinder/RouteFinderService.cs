using System.Collections.Generic;
using System.Linq;
using DatabaseContext.DatabaseAcces;
using DatabaseContext.Domain;
using Services.RouteFinder.Dto;
using System;

namespace Services.RouteFinder
{
    public class RouteFinderService : IRouteFinderService
    {
        private readonly IRepository<TrainHistoryData> _trainHistoryDataRepository;
        private readonly IRepository<Tren> _trenRepository;
        private readonly IRepository<Trasa> _trasaRepository;
        private readonly IRepository<Statie> _statieRepository;
        private List<List<int>> adjLists;
        private Dictionary<int, int> indexes;

        public RouteFinderService(IRepository<TrainHistoryData> trainHistoryDataRepository, IRepository<Tren> trenRepository, 
            IRepository<Trasa> trasaRepository, IRepository<Statie> statieRepository)
        {
            _trainHistoryDataRepository = trainHistoryDataRepository;
            _trenRepository = trenRepository;
            _trasaRepository = trasaRepository;
            _statieRepository = statieRepository;
            BuildAdjList();
        }

        public AllRoutesResultsDto FindRoute(RouteFinderInputDto route)
        {
            var routeOutput = new AllRoutesResultsDto();
            routeOutput.Routes = new List<RouteDto>();

            if (route.OraPlecare.HasValue)
            {
                int oraPlecare = (int)route.OraPlecare.Value.TotalSeconds;
                for(var index=1; index<route.StationToGoThrow.Count(); index++)
                {
                    var path = ComputeRoutes3(route.StationToGoThrow[index-1].StationCode,
                        route.StationToGoThrow[index].StationCode, oraPlecare, 
                        route.StationToSkip, route.YearIdentifier);
                    if (path != null)
                    {
                        var auxRoute = GetRouteDtoFromTrasa(path);
                        routeOutput.Routes.Add(auxRoute);
                        oraPlecare = auxRoute.Points.LastOrDefault().OraAjungere + route.StationToGoThrow[index].SencondsToWait;
                    }
                }
                
            }

            return routeOutput;
        }

        RouteDto GetRouteDtoFromTrasa(List<Trasa> oldRoute)
        {
            List<PointDto> points = new List<PointDto>();

            Trasa aux = oldRoute[0];
            Statie statie = GetStationWithCod(aux.CodStaOrigine);
            var point = new PointDto
            {
                StatieNume = aux.DenStaOrigine,
                CodStatie = aux.CodStaOrigine,
                OraAjungere = aux.OraPlecare,
                Position = statie.LatLong
            };
            points.Add(point);

            foreach(Trasa trasa in oldRoute)
            {
                statie = GetStationWithCod(trasa.CodStaDest);
                point = new PointDto
                {
                    StatieNume = trasa.DenStaDest,
                    CodStatie = trasa.CodStaDest,
                    OraAjungere = trasa.OraSosire,
                    Position = statie.LatLong
                };
                points.Add(point);
            }
            RouteDto newRoute = new RouteDto { Points = points };

            return newRoute;
        }

        private List<Trasa> ComputeRoutes3(int startCode, int destCode, int startHour, List<int> stationsToSkip, 
            string yearIdentifier)
        {
            List<Trasa> routes = null;
            List<int> visited = new List<int>();
            List<List<int>> paths = new List<List<int>>();
            int count = 0;
            
            DFS3(startCode, destCode, ref count, ref paths, ref visited);
           
            List<int> minPath = null;
            if (paths.Count() > 0)
            {
                //cum fol stationsToSkip?
                minPath = paths[0];
                for (int i = 1; i < paths.Count(); i++)
                {
                    var flag = false;
                    foreach (int skipCode in stationsToSkip)
                    {
                        if (paths[i].Contains(skipCode))
                            flag = true;
                    }
                    if (!flag && minPath.Count() > paths[i].Count())
                        minPath = paths[i];
                }
                    
                //deocamdata returnez minimul dar s-ar putea sa fie nevoie sa le returnez pe toate !!!!!!!!!!!1
                routes = GetRoute(minPath, startHour, yearIdentifier);
            }
            
            return routes;
        }

        private void DFS3(int start, int goal, ref int count, ref List<List<int>> paths, ref List<int> visited)
        {
            visited.Add(start);
            if (visited.Count() < 30)
            {
                if (start == goal)
                {
                    List<int> aux = new List<int>();
                    aux.AddRange(visited);
                    paths.Add(aux);
                }
                else
                {
                    var aux = adjLists[indexes[start]];
                    var pos = 0;
                    int cod;
                    while (pos < aux.Count())
                    {
                        cod = aux[pos];
                        if (!visited.Contains(cod))
                        {
                            count++;
                            DFS3(cod, goal, ref count, ref paths, ref visited);
                        }
                        pos++;
                    }
                }
            }
                
            visited.Remove(start);
        }

        private List<Trasa> GetRoute(List<int> path, int startHour, string yearIdentifier)
        {
            //fol drumul gasit cauta o lista de trase coresp
            var route = new List<Trasa>();
            int codStart = path[0];
            int codDest;
            for (int i = 1; i < path.Count(); i++)
            {
                codDest = path[i];
                var trasa = GetTrasa(codStart, codDest, startHour, yearIdentifier);
                //startHour += trasa.OraSosire - trasa.OraPlecare;
                startHour = trasa.OraSosire;
                route.Add(trasa);
                codStart = codDest;
            }
            return route;
        }

        private Trasa GetTrasa(int codStart, int codDest, int startHour, string yearIdentifier)
        {
            List<Trasa> auxList = new List<Trasa>();
            //var trase = _trasaRepository.GetQuery(x => x.YearIdentifier == DateTime.Now.Year.ToString());
            var trase = _trasaRepository.GetQuery(x => x.YearIdentifier == yearIdentifier);
            foreach (var trasaEntitate in trase.Where(x => x.CodStaOrigine == codStart && x.CodStaDest == codDest))
            {
                auxList.Add(trasaEntitate);
            }

            var trasa = auxList[0];
            for (int i = 1; i < auxList.Count(); i++)
            {
                var conditionIsMet = auxList[i].OraPlecare - startHour >= 0;
                if (trasa.OraPlecare - startHour >= 0)
                    conditionIsMet &= auxList[i].OraPlecare < trasa.OraPlecare;
                trasa = conditionIsMet ? auxList[i] : trasa;
            }
            return trasa;
        }

        //construiesc listele de adiacenta pentru statii/noduri
        private void BuildAdjList()
        {
            int pos = 0;
            adjLists = new List<List<int>>();
            indexes = new Dictionary<int, int>();
            var stations = _statieRepository.GetAll();
            var trase = _trasaRepository.GetQuery(x => x.YearIdentifier == DateTime.Now.Year.ToString());
            foreach (Statie statie in stations)
            {
                List<int> aux = new List<int>();

                foreach (var trasa in trase.Where(x => x.CodStaOrigine == statie.CodStatie))
                {
                    if (!aux.Contains(trasa.CodStaDest))
                        aux.Add(trasa.CodStaDest);

                }
                adjLists.Add(aux);
                indexes.Add(statie.CodStatie, pos++);
            }
        }

        public List<StatieDto> GetStationThatHaveNameWithTerm(string term)
        {
            var stations =
              (from station in _statieRepository.GetQuery(x => (x.NumeStatie).ToLower().Contains(term.ToLower()))
               select new StatieDto { CodStatie = station.CodStatie, NumeStatie = station.NumeStatie }).ToList();
            return stations;
        }

        private Statie GetStationWithCod(int cod)
        {
            var stations = _statieRepository.GetAll();
            foreach (Statie statie in stations)
                if (statie.CodStatie == cod)
                    return statie;
            //stations.First(x => x.CodStatie == cod);
            return null;
        }
    }
}
