using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseContext.Domain
{
    public class Tren:DomainBase
    {
        public string CategorieTren { get; set; }
        public double KmCum { get; set; }
        public int Lungime { get; set; }
        public string Numar { get; set; }
        public string Proprietar { get; set; }
        public string Operator { get; set; }
        public string Putere { get; set; }
        public int Rang { get; set; }
        public int Servicii { get; set; }
        public int Tonaj { get; set; }
        public int StatieInitiala { get; set; }
        public int StatieFinala { get; set; }
        public int TraseuId { get; set; }
        public string Tip { get; set; }
        public DateTime CalendarDeLa { get; set; }
        public DateTime CalendarPanaLa { get; set; }
        public int Zile { get; set; }
        public string YearIdentifier { get; set; }
    }
}
