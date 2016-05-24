using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TroW.Models
{
    public class Account
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Password1 { get; set; }
        public bool Register { get; set; }
        public bool Login { get; set; }
        public bool IsFacebook { get; set; }
        public bool ForgatEmail { get; set; }
        public bool ResetPassword { get; set; }
        public string IdFacebook { get; set; }
        public Guid ForgatPasswordGuid { get; set; }
    }
}