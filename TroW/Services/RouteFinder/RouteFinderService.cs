using DatabaseContext.DatabaseAcces;
using DatabaseContext.Domain;
using Services.RouteFinder.Dto;

namespace Services.RouteFinder
{
    class RouteFinderService
    {
        private readonly IRepository<TrainHistoryData> _trainHistoryDataRepository;
        private readonly IRepository<Tren> _trenRepository;
        private readonly IRepository<Trasa> _trasaRepository;
        public RouteFinderService(IRepository<TrainHistoryData> trainHistoryDataRepository, IRepository<Tren> trenRepository, IRepository<Trasa> trasaRepository)
        {
            _trainHistoryDataRepository = trainHistoryDataRepository;
            _trenRepository = trenRepository;
            _trasaRepository = trasaRepository;
        }

        public RouteFinderOutputDto FindRoute(RouteFinderInputDto route)
        {
            return new RouteFinderOutputDto();
        }
    }
}
