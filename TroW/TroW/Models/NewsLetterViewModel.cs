using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TroW.Models
{
    public class NewsLetterViewModel
    {
        public Guid? Id { get; set; }
        public Guid? IdClone { get; set; }
        public string Header { get; set; }
        public string Message { get; set; }
    }
}