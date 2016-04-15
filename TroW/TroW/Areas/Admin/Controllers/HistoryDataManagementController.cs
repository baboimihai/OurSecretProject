using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Services.HistoryDataManagement;


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
            _dataManagementService.UploadData(@"C:\2016.xml","2016","2016");
            return View();
        }

    }
}
