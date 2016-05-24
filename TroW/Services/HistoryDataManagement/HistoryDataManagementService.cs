using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using DatabaseContext.DatabaseAcces;
using DatabaseContext.Domain;
using MoreLinq;
using Services.HistoryDataManagement.Dto;
using Services.RouteFinder;
using Services.RouteFinder.Dto;

namespace Services.HistoryDataManagement
{
    public class HistoryDataManagementService : IHistoryDataManagementService
    {
        private readonly IRepository<TrainHistoryData> _trainHistoryDataRepository;
        private readonly IRepository<Tren> _trenRepository;
        private readonly IRepository<Trasa> _trasaRepository;
        private readonly IRepository<Statie> _statieRepository;
        private readonly IRepository<NewsLetter> _newsLetterRepository;
        private readonly IRepository<SearchedHistory> _searchedHistoryRepository;
        private readonly IRepository<SearchedHistoryLocations> _searchedHistoryLocationsRepository;
        private readonly IRouteFinderService _routeFinderService;
        public HistoryDataManagementService(IRepository<TrainHistoryData> trainHistoryDataRepository, IRepository<NewsLetter> newsLetterRepository,
            IRepository<Tren> trenRepository, IRepository<Trasa> trasaRepository, IRepository<Statie> statieRepository,
            IRepository<SearchedHistory> searchedHistoryRepository, IRepository<SearchedHistoryLocations> searchedHistoryLocationsRepository, IRouteFinderService routeFinderService)
        {
            _trainHistoryDataRepository = trainHistoryDataRepository;
            _trenRepository = trenRepository;
            _trasaRepository = trasaRepository;
            _statieRepository = statieRepository;
            _newsLetterRepository = newsLetterRepository;
            _searchedHistoryRepository = searchedHistoryRepository;
            _searchedHistoryLocationsRepository = searchedHistoryLocationsRepository;
            _routeFinderService = routeFinderService;
        }

        public void UploadData(string fileUrl, string name, string yearIdentifier)
        {
            byte[] data;
            using (WebClient webClient = new WebClient())
                data = webClient.DownloadData(fileUrl);
            string str = Encoding.GetEncoding("Windows-1252").GetString(data);
            var xmlX = new XmlDocument();
            xmlX.LoadXml(str);
            var xml = new XDocument();
            var nodeReader = new XmlNodeReader(xmlX);

            nodeReader.MoveToContent();
            xml = XDocument.Load(nodeReader);

            var trenuriQuerry = (from trenuri in xml.Descendants("Trenuri")
                                 from tren in trenuri.Elements()
                                 select new TrenContainerDto
                                 {
                                     CategorieTren = tren.Attribute("CategorieTren").Value,
                                     KmCum = Double.Parse(tren.Attribute("KmCum").Value),
                                     Lungime = Int32.Parse(tren.Attribute("Lungime").Value),
                                     Numar = tren.Attribute("Numar").Value,
                                     Proprietar = tren.Attribute("Proprietar").Value,
                                     Operator = tren.Attribute("Operator").Value,
                                     Putere = tren.Attribute("Putere").Value,
                                     Rang = Int32.Parse(tren.Attribute("Rang").Value),
                                     Servicii = Int32.Parse(tren.Attribute("Servicii").Value),
                                     Tonaj = Int32.Parse(tren.Attribute("Tonaj").Value),
                                     StatieFinala = Int32.Parse(tren.Descendants("Trasa").First().Attribute("CodStatieFinala").Value),
                                     StatieInitiala = Int32.Parse(tren.Descendants("Trasa").First().Attribute("CodStatieInitiala").Value),
                                     Tip = tren.Descendants("Trasa").First().Attribute("Tip").Value,
                                     TraseuId = Int32.Parse(tren.Descendants("Trasa").First().Attribute("Id").Value),
                                     CalendarDeLa = DateTime.ParseExact(tren.Descendants("RestrictiiTren").First().Descendants("CalendarTren").First().Attribute("DeLa").Value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None),
                                     CalendarPanaLa = DateTime.ParseExact(tren.Descendants("RestrictiiTren").First().Descendants("CalendarTren").First().Attribute("PinaLa").Value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None),
                                     Zile = Int32.Parse(tren.Descendants("RestrictiiTren").First().Descendants("CalendarTren").First().Attribute("Zile").Value),
                                     Trase = (from trasa in tren.Descendants("Trase").First().Descendants("Trasa").Elements()
                                              where trasa.Attribute("Ajustari") != null
                                              select new TrasaDto
                                           {
                                               Ajustari = Int32.Parse(trasa.Attribute("Ajustari").Value),
                                               CodStaDest = Int32.Parse(trasa.Attribute("CodStaDest").Value),
                                               CodStaOrigine = Int32.Parse(trasa.Attribute("CodStaOrigine").Value),
                                               DenStaDest = trasa.Attribute("DenStaDestinatie").Value,
                                               DenStaOrigine = trasa.Attribute("DenStaOrigine").Value,
                                               Km = Int32.Parse(trasa.Attribute("Km").Value),
                                               TipOprire = trasa.Attribute("TipOprire").Value,
                                               Tonaj = Int32.Parse(trasa.Attribute("Tonaj").Value),
                                               VitezaLivret = Int32.Parse(trasa.Attribute("VitezaLivret").Value),
                                               StationareSecunde = Int32.Parse(trasa.Attribute("StationareSecunde").Value),
                                               Secventa = Int32.Parse(trasa.Attribute("Secventa").Value),
                                               Restrictie = Int32.Parse(trasa.Attribute("Restrictie").Value),
                                               Lungime = Int32.Parse(trasa.Attribute("Lungime").Value),
                                               Rci = trasa.Attribute("Rci").Value,
                                               Rco = trasa.Attribute("Rco").Value,
                                               OraPlecare = Int32.Parse(trasa.Attribute("OraP").Value),
                                               OraSosire = Int32.Parse(trasa.Attribute("OraS").Value),
                                               YearIdentifier = yearIdentifier
                                           }).ToList(),
                                     YearIdentifier = yearIdentifier

                                 }

                ).ToList();
            var trase = trenuriQuerry.SelectMany(x => x.Trase).ToList();
            var statii = (from s in trase select new Statie { CodStatie = s.CodStaDest, NumeStatie = s.DenStaDest }).ToList().DistinctBy(d => d.CodStatie).ToList();
            var existingStation = _statieRepository.GetAll();
            statii.ForEach(statie =>
            {
                if (existingStation.SingleOrDefault(x => x.CodStatie == statie.CodStatie) == null)
                    _statieRepository.Add(statie);
            }
                );
            foreach (var tren in trenuriQuerry)
            {
                var trenEntity = (Tren)tren;
                trenEntity.YearIdentifier = yearIdentifier;
                _trenRepository.Add(trenEntity);
                var traseEntities = (from x in tren.Trase select x).ToList();
                traseEntities.ForEach(trasaDto =>
                {
                    var trasa = (Trasa)trasaDto;
                    trasa.TrenId = trenEntity.Id;
                    _trasaRepository.Add(trasa);
                });
            }
            var upload = new TrainHistoryData
            {
                DateUploaded = DateTime.Now,
                IsActive = true,
                Name = name,
                YearIdentifier = yearIdentifier
            };
            _trainHistoryDataRepository.Add(upload);
        }

        public List<StatieDto> GetAllAstationsDtos()
        {
            return (from x in _statieRepository.GetQuery(x => !x.IsInactive)
                    select new StatieDto
                    {
                        CodStatie = x.CodStatie,
                        LatLong = x.LatLong,
                        NumeStatie = x.NumeStatie
                    }).ToList();
        }

        public void SaveAllStationsLocations(StatieDto statiiDtos)
        {
            var stationEntity = _statieRepository.GetQuery(x => x.CodStatie == statiiDtos.CodStatie).First();
            stationEntity.LatLong = statiiDtos.LatLong;
            _statieRepository.Update(stationEntity);
        }

        public List<NewsLetterDto> GetAllNews()
        {
            return
                (from x in _newsLetterRepository.GetAll()
                 select new NewsLetterDto { Header = x.Header, Id = x.Id, Message = x.Message }).ToList();
        }

        public void AddNews(NewsLetterDto newsLetterDto)
        {
            if (!newsLetterDto.Id.HasValue)
            {
                _newsLetterRepository.Add(new NewsLetter
                {
                    Header = newsLetterDto.Header,
                    Message = newsLetterDto.Message
                });
            }

            else
            {
                var newsLetterEntity = _newsLetterRepository.GetById(newsLetterDto.Id.Value);
                newsLetterEntity.Header = newsLetterDto.Header;
                newsLetterEntity.Message = newsLetterDto.Message;
                _newsLetterRepository.Update(newsLetterEntity);
            }
        }

        public void DeleteNews(Guid id)
        {
            var newsLetterEntity = _newsLetterRepository.GetById(id);
            if (newsLetterEntity != null)
            {
                _newsLetterRepository.Delete(newsLetterEntity);
            }
        }
        public NewsLetterDto GetNewsById(Guid id)
        {
            var newsLetterEntity = _newsLetterRepository.GetById(id);
            if (newsLetterEntity != null)
            {
                return new NewsLetterDto { Message = newsLetterEntity.Message, Header = newsLetterEntity.Header, Id = newsLetterEntity.Id };
            }
            return new NewsLetterDto();
        }
        public void SetInactiveLocation(int codStatie)
        {
            var newsLetterEntity = _statieRepository.GetQuery(x => x.CodStatie == codStatie).First();
            if (newsLetterEntity != null)
            {
                newsLetterEntity.IsInactive = true;
                _statieRepository.Update(newsLetterEntity);
            }
        }
        public StatieDto GetLocationByCode(int codStatie)
        {
            var newsLetterEntity = _statieRepository.GetQuery(x => x.CodStatie == codStatie).First();
            if (newsLetterEntity != null)
            {
                return new StatieDto { CodStatie = newsLetterEntity.CodStatie, NumeStatie = newsLetterEntity.NumeStatie, LatLong = newsLetterEntity.LatLong };
            }
            return new StatieDto();
        }
        public void UpdateStation(StatieDto newsLetterDto)
        {

            var stationEntity = _statieRepository.GetQuery(x => x.CodStatie == newsLetterDto.CodStatie).First();
            if (stationEntity != null)
            {
                stationEntity.NumeStatie = newsLetterDto.NumeStatie;
                stationEntity.LatLong = newsLetterDto.LatLong;
                _statieRepository.Update(stationEntity);
            }
        }

        public void SaveUserSearch(RouteFinderInputDto searchInputDto)
        {
            var dateTime = DateTime.Now;
            var guid = Guid.NewGuid();
            var enitityAded = _searchedHistoryRepository.Add(new SearchedHistory
            {
                CustomerId = searchInputDto.UserId,
                Time = dateTime,
                DataPlecare = searchInputDto.DataPlecare,
                PlecareCuOra = searchInputDto.OraPlecare,
                PlecarePanaLa = searchInputDto.OraSosire,
            }, guid);

            foreach (var i in searchInputDto.StationToGoThrow)
            {
                _searchedHistoryLocationsRepository.Add(new SearchedHistoryLocations
                {
                    CodStatie = i.StationCode,
                    SencondsToWait = i.SencondsToWait,
                    SearchedHistoryId = guid,
                    Type = (int)SearchedHistoryLocationsType.Via
                });
            }
            foreach (var i in searchInputDto.StationToSkip)
            {
                _searchedHistoryLocationsRepository.Add(new SearchedHistoryLocations
                {
                    CodStatie = i,
                    SearchedHistoryId = guid,
                    Type = (int)SearchedHistoryLocationsType.Exclude
                });
            }

        }

        public RouteFinderInputDto GetUserSearch(Guid id)
        {
            var inputSearched = new RouteFinderInputDto();
            var searchedHistory = _searchedHistoryRepository.GetById(id);
            inputSearched.OraPlecare = searchedHistory.PlecareCuOra;
            inputSearched.OraSosire = searchedHistory.PlecarePanaLa;
            inputSearched.DataPlecare = searchedHistory.DataPlecare;
            inputSearched.StationToGoThrow = (from x in _searchedHistoryLocationsRepository.GetQuery(
                    x => x.SearchedHistoryId == searchedHistory.Id && x.Type == (int)SearchedHistoryLocationsType.Via) select new ViaPlaningDto{SencondsToWait = x.SencondsToWait,StationCode = x.CodStatie}).ToList();

            inputSearched.StationToSkip =
    _searchedHistoryLocationsRepository.GetQuery(
        x => x.SearchedHistoryId == searchedHistory.Id && x.Type == (int)SearchedHistoryLocationsType.Exclude)
        .Select(x => x.CodStatie)
        .ToList();
            return inputSearched;
        }
        public List<UserHistorySearchedRecordDto> GetUserHistory(Guid userId)
        {
            var searchedSettings = (from x in _searchedHistoryRepository.GetQuery(x => x.CustomerId == userId)
                                    select new UserHistorySearchedRecordDto
                                    {
                                        Id = x.Id,
                                        DataPlecare = x.DataPlecare,
                                        PlecarePanaLa = x.PlecarePanaLa,
                                        PlecareCuOra = x.PlecareCuOra,
                                        Time = x.Time,
                                    }).ToList();
            var allSearchedIds = searchedSettings.Select(x => x.Id).ToList();
            var allSearchedLocations =
                (from x in
                     _searchedHistoryLocationsRepository.GetQuery(x => allSearchedIds.Contains(x.SearchedHistoryId))
                 select new SearchedHistoryLocationsDto
                 {
                     SearchedHistoryId = x.SearchedHistoryId,
                     CodStatie = x.CodStatie,
                     Type = x.Type,
                     SencondsToWait = x.SencondsToWait
                 }).ToList();
            var allLocationsCodes = allSearchedLocations.Select(x => x.CodStatie).ToList();
            var allLocationsEntities = _statieRepository.GetQuery(x => allLocationsCodes.Contains(x.CodStatie)).ToList();
            allSearchedLocations.ForEach(station =>
            {
                station.NumeStatie = allLocationsEntities.Single(x => x.CodStatie == station.CodStatie).NumeStatie;
            });
            searchedSettings.ForEach(searchedSetting =>
            {
                searchedSetting.SearchedLocations =
                    allSearchedLocations.Where(x => x.SearchedHistoryId == searchedSetting.Id).ToList();
            });
            return searchedSettings;
        }

        public List<TrainHistoryDataDto> GetAllTrainData()
        {
            return (from x in _trainHistoryDataRepository.GetAll() select new TrainHistoryDataDto { YearIdentifier = x.YearIdentifier, DateUploaded = x.DateUploaded, IsActive = x.IsActive, Name = x.Name }).ToList();
        }

        public void ChangeYearStatus(string yearIdentifier)
        {
            var entityStatus = _trainHistoryDataRepository.GetQuery(x => x.YearIdentifier == yearIdentifier).First();
            entityStatus.IsActive = !entityStatus.IsActive;
            _trainHistoryDataRepository.Update(entityStatus);

        }

        public List<StatisticsDto> GetStatistics(RouteFinderInputDto input)
        {
            var statistics = new List<StatisticsDto>();
            var dataHistory=GetAllTrainData();
            var dataSearched = new List<AllRoutesResultsDto>();
            dataHistory.ForEach(data =>
            {
                statistics.Add(new StatisticsDto{YearIdentifier = data.YearIdentifier});
                input.YearIdentifier = data.YearIdentifier;
                dataSearched.Add(_routeFinderService.FindRoute(input));
            });
            for (int i = 0; i < dataSearched.Count; i++)
            {
                statistics[i].TotalDuration = new List<RouteDuration>();
                statistics[i].TotalPartialDuration = new List<RoutePartialDuration>();
                dataSearched[i].Routes.ForEach(route =>
                {
                    statistics[i].TotalDuration.Add(new RouteDuration
                    {
                        Duration = route.Points.Last().OraAjungere - route.Points.First().OraAjungere
                    });
                    for (int j = 0; j < route.Points.Count - 1; j++)
                    {
                        statistics[i].TotalPartialDuration.Add(new RoutePartialDuration
                        {
                            StatiePlecare = route.Points[j].StatieNume,
                            StatieDestinatie = route.Points[j+1].StatieNume,
                            Duration = route.Points[j+1].OraAjungere - route.Points[j].OraAjungere
                        }); 
                    }
 
                });
            }
            return statistics;
        }
    }
}
