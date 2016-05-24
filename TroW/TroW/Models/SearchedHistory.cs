using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TroW.Models
{
    public class SearchedHistory
    {
        public Guid Id { get; set; }
        public DateTime? Time { get; set; }
        public DateTime? DataPlecare { get; set; }
        public TimeSpan? PlecareCuOra { get; set; }
        public TimeSpan? PlecarePanaLa { get; set; }
        public List<SearchedHistoryLocations> SearchedLocations { get; set; }
        public String TimeString { get; set; }
        public String DataPlecareString { get; set; }
    }
    public class SearchedHistoryLocations
    {
        public string SencondsToWait { get; set; }
        public string NumeStatie { get; set; }
        public int Type { get; set; }
    }
}