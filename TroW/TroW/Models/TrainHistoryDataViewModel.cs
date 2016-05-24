using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TroW.Models
{
    public class TrainHistoryDataViewModel
    {
        public string Name { get; set; }
        public DateTime DateUploaded { get; set; }
        public bool IsActive { get; set; }
        public string YearIdentifier { get; set; }
    }
}