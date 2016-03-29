using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseContext.Domain;

namespace Services.HistoryDataManagement
{
    public class TrenContainerDto:Tren
    {
        public List<Trasa> Trase { get; set; } 
    }
}
