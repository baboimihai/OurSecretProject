using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DatabaseContext.DatabaseAcces;
using DatabaseContext.Domain;

namespace Services.HistoryDataManagement
{
    public class HistoryDataManagementService:IHistoryDataManagementService
    {
        private readonly IRepository<TrainHistoryData> _trainHistoryDataRepository;
        private readonly IRepository<Tren> _trenRepository;
        private readonly IRepository<Trasa> _trasaRepository;
        public HistoryDataManagementService(IRepository<TrainHistoryData> trainHistoryDataRepository, IRepository<Tren> trenRepository, IRepository<Trasa> trasaRepository)
        {
            _trainHistoryDataRepository = trainHistoryDataRepository;
            _trenRepository = trenRepository;
            _trasaRepository = trasaRepository;
        }

        public void UploadData(string fileName,string name,string yearIdentifier)
        {
            var xml = XDocument.Load(fileName);
            var trenuriQuerry = (from trenuri in xml.Descendants("Trenuri")
                                 from tren in trenuri.Elements()
                                 select new TrenContainerDto
                                 {
                                     CategorieTren = tren.Attribute("CategorieTren").Value,
                                     KmCum = Double.Parse(tren.Attribute("KmCum").Value),
                                     Lungime = Int32.Parse(tren.Attribute("Lungime").Value),
                                     Numar = tren.Attribute("Numar").Value,
                                     Proprietar = tren.Attribute("Proprietar").Value,
                                     Operator = tren.Attribute("Operator").Value,
                                     Putere = tren.Attribute("Putere").Value,
                                     Rang = Int32.Parse(tren.Attribute("Rang").Value),
                                     Servicii = Int32.Parse(tren.Attribute("Servicii").Value),
                                     Tonaj = Int32.Parse(tren.Attribute("Tonaj").Value),
                                     StatieFinala = Int32.Parse(tren.Descendants("Trasa").First().Attribute("CodStatieFinala").Value),
                                     StatieInitiala = Int32.Parse(tren.Descendants("Trasa").First().Attribute("CodStatieInitiala").Value),
                                     Tip = tren.Descendants("Trasa").First().Attribute("Tip").Value,
                                     TraseuId = Int32.Parse(tren.Descendants("Trasa").First().Attribute("Id").Value),
                                     CalendarDeLa = DateTime.ParseExact(tren.Descendants("RestrictiiTren").First().Descendants("CalendarTren").First().Attribute("DeLa").Value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None),
                                     CalendarPanaLa = DateTime.ParseExact(tren.Descendants("RestrictiiTren").First().Descendants("CalendarTren").First().Attribute("PinaLa").Value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None),
                                     Zile = Int32.Parse(tren.Descendants("RestrictiiTren").First().Descendants("CalendarTren").First().Attribute("Zile").Value),
                                     Trase = (from trasa in tren.Descendants("Trase").First().Descendants("Trasa").Elements()
                                              where trasa.Attribute("Ajustari") != null
                                              select new Trasa
                                              {
                                                  Ajustari = Int32.Parse(trasa.Attribute("Ajustari").Value),
                                                  CodStaDest = Int32.Parse(trasa.Attribute("CodStaDest").Value),
                                                  CodStaOrigine = Int32.Parse(trasa.Attribute("CodStaOrigine").Value),
                                                  Km = Int32.Parse(trasa.Attribute("Km").Value),
                                                  TipOprire = trasa.Attribute("TipOprire").Value,
                                                  Tonaj = Int32.Parse(trasa.Attribute("Tonaj").Value),
                                                  VitezaLivret = Int32.Parse(trasa.Attribute("VitezaLivret").Value),
                                                  StationareSecunde = Int32.Parse(trasa.Attribute("StationareSecunde").Value),
                                                  Secventa = Int32.Parse(trasa.Attribute("Secventa").Value),
                                                  Restrictie = Int32.Parse(trasa.Attribute("Restrictie").Value),
                                                  Lungime = Int32.Parse(trasa.Attribute("Lungime").Value),
                                                  Rci = trasa.Attribute("Rci").Value,
                                                  Rco = trasa.Attribute("Rco").Value,
                                                  OraPlecare = Int32.Parse(trasa.Attribute("OraP").Value),
                                                  OraSosire = Int32.Parse(trasa.Attribute("OraS").Value),
                                                  YearIdentifier = yearIdentifier
                                              }).ToList(),
                                     YearIdentifier = yearIdentifier

                                 }
                                  
                ).ToList();

            foreach (var tren in trenuriQuerry)
            {
                var trenEntity =(Tren) tren;
                _trenRepository.Add(trenEntity);
                var traseEntities = (from x in tren.Trase select x).ToList();
                traseEntities.ForEach(trasa=>
                    _trasaRepository.Add(trasa)
                    );
            }
            var upload=new TrainHistoryData
            {
                DateUploaded = DateTime.Now,
                IsActive = true,
                Name = name,
                YearIdentifier = yearIdentifier
            };
            _trainHistoryDataRepository.Add(upload);
        }
    }
}
