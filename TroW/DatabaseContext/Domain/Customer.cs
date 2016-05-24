using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseContext.DatabaseAcces;

namespace DatabaseContext.Domain
{
    public class Customer : DomainBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public string FacebookId { get; set; }
        public Guid ForgatPasswordGuid { get; set; }
    }
}
