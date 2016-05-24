using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.HistoryDataManagement
{
    public class NewsLetterDto
    {
        public Guid? Id { get; set; }
        public string Header { get; set; }
        public string Message { get; set; }
    }
}
