using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Services.Customer;

namespace TroW.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ICustomerServices customerServices;

        public HomeController(ICustomerServices customerServices)
        {
            this.customerServices = customerServices;
        }//
        
      //  public readonly CustomerService CustomerService;

    //    public HomeController(CustomerService customerService)
   //     {
    //        CustomerService = customerService;
  //      }

        public ActionResult Index()
        {
            var x = customerServices.GetEmail();
            ViewBag.Message = "aaaModify this template to jump-start your ASP.NET MVC application.aaaaaaaa";
          //  var emails = CustomerService.GetEmail();
          //  ViewBag.Message = emails.Count();
            return View(x);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
