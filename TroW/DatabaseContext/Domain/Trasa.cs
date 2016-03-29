using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseContext.Domain
{
    public class Trasa:DomainBase
    {
        public int Ajustari { get; set; }
        public int CodStaDest { get; set; }
        public int CodStaOrigine { get; set; }
        public int Km { get; set; }
        public int Lungime { get; set; }
        public string TipOprire { get; set; }
        public int Tonaj { get; set; }
        public int VitezaLivret { get; set; }
        public int StationareSecunde { get; set; }
        public int Secventa { get; set; }
        public int Restrictie { get; set; }
        public string Rci { get; set; }
        public string Rco { get; set; }
        public int OraPlecare { get; set; }
        public int OraSosire { get; set; }
        public string YearIdentifier { get; set; }
    }
}
