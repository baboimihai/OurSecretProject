using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.HistoryDataManagement.Dto;
using Services.RouteFinder.Dto;

namespace Services.HistoryDataManagement
{
    public interface IHistoryDataManagementService
    {
        void UploadData(string fileUrl, string name, string yearIdentifier);
        List<StatieDto> GetAllAstationsDtos();
        void SaveAllStationsLocations(StatieDto statiiDtos);
        List<NewsLetterDto> GetAllNews();
        void AddNews(NewsLetterDto newsLetterDto);
        void DeleteNews(Guid id);
        NewsLetterDto GetNewsById(Guid id);
        void SetInactiveLocation(int codStatie);
        StatieDto GetLocationByCode(int codStatie);
        void UpdateStation(StatieDto newsLetterDto);
        void SaveUserSearch(RouteFinderInputDto searchInputDto);
        List<UserHistorySearchedRecordDto> GetUserHistory(Guid userId);
        RouteFinderInputDto GetUserSearch(Guid id);
        List<TrainHistoryDataDto> GetAllTrainData();
        void ChangeYearStatus(string yearIdentifier);
        List<StatisticsDto> GetStatistics(RouteFinderInputDto input);
    }
}
