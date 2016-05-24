using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseContext.Domain
{
    public class NewsLetter:DomainBase
    {
        public string Header { get; set; }
        public string Message { get; set; }
    }
}
