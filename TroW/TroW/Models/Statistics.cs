using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Services.HistoryDataManagement.Dto;

namespace TroW.Models
{
    public class Statistics
    {
        public string YearIdentifier { get; set; }
        public List<RouteDurationViewModel> TotalDuration { get; set; }
    }
        public class RouteDurationViewModel
    {
        public string Duration { get; set; }
    }

    public class RoutePartialDurationViewModel 
    {
        public List<string> YearResultDuration { get; set; }
        public string StatiePlecare { get; set; }
        public string StatieDestinatie { get; set; }
        public string YearIdentifier { get; set; }
        public string Duration { get; set; }
    }
}
