using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.HistoryDataManagement
{
    public interface IHistoryDataManagementService
    {
        void UploadData(string fileName, string name, string yearIdentifier);
        List<StatieDto> GetAllAstationsDtos();
        void SaveAllStationsLocations(List<StatieDto> statiiDtos);
    }
}
