using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Services.HistoryDataManagement;
using TroW.Areas.Admin.Models;
using TroW.Models;


namespace TroW.Areas.Admin.Controllers
{
    public class HistoryDataManagementController : Controller
    {
        //
        // GET: /Admin/HistoryDataManagement/
        private readonly IHistoryDataManagementService _dataManagementService;
        public HistoryDataManagementController(IHistoryDataManagementService dataManagementService)
        {
            _dataManagementService = dataManagementService;
        }

        public ActionResult Index()
        {
            //_dataManagementService.UploadData(@"C:\2016.xml","2016","2016");
            //_dataManagementService.UploadData(@"C:\2015.xml", "2015", "2015");
            //_dataManagementService.UploadData(@"C:\2014.xml", "2014", "2014");
            return View();
        }

        public ActionResult UpdateStationsPositions()
        {
            PopulateNewsLetters();
            var statii = (from x in _dataManagementService.GetAllAstationsDtos()
                select new StatieViewModel {CodStatie = x.CodStatie, NumeStatie = x.NumeStatie}).ToList();
            return View();
        }
        List<TroW.Models.NewsLetterViewModel> PopulateNewsLetters()
        {
            var newsLetters = new List<NewsLetterViewModel>();
            newsLetters.Add(new NewsLetterViewModel { Header = "Schimbare de tren", Message = "ceva tetxt aici de umplutura" });
            newsLetters.Add(new NewsLetterViewModel { Header = "Schimbare de tren", Message = "ceva tetxt aici de umplutura" });
            newsLetters.Add(new NewsLetterViewModel { Header = "Schimbare de tren", Message = "ceva tetxt aici de umplutura" });
            newsLetters.Add(new NewsLetterViewModel { Header = "Schimbare de tren", Message = "ceva tetxt aici de umplutura" });
            newsLetters.Add(new NewsLetterViewModel { Header = "Schimbare de tren", Message = "ceva tetxt aici de umplutura" });
            newsLetters.Add(new NewsLetterViewModel { Header = "Schimbare de tren", Message = "ceva tetxt aici de umplutura" });
            newsLetters.Add(new NewsLetterViewModel { Header = "Schimbare de tren", Message = "ceva tetxt aici de umplutura" });
            newsLetters.Add(new NewsLetterViewModel { Header = "Schimbare de tren", Message = "ceva tetxt aici de umplutura" });
            return newsLetters;
        }

    }
}
