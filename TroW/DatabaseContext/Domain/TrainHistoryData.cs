using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseContext.Domain
{
    public class TrainHistoryData:DomainBase
    {
        public string Name { get; set; }
        public DateTime DateUploaded { get; set; }
        public bool IsActive { get; set; }
        public string YearIdentifier { get; set; }
    }
}
