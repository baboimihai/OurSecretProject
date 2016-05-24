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
        public List<RouteDuration> TotalDuration { get; set; }
        public List<RoutePartialDuration> TotalPartialDuration { get; set; }
    }

}