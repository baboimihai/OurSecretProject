using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using Services.CustomersService;
using Services.CustomerService;
using Services.HistoryDataManagement;
using TroW.Identity;
using TroW.Models;

namespace TroW.Controllers
{
     [UserAuthorize]
    public class AccountController : Controller
    {
         private readonly ICustomerServices _customerServices;
         private readonly IHistoryDataManagementService _historyDataManagementService;

         public AccountController(ICustomerServices customerServices,IHistoryDataManagementService historyDataManagementService)
         {
             _customerServices=customerServices;
             _historyDataManagementService = historyDataManagementService;
         }
         public ActionResult UserSearchHistory()
         {
             var searchedResults =
                 (from x in _historyDataManagementService.GetUserHistory(Identity.Identity.Current.Customer.Id)
                     select new SearchedHistory
                     {
                         Id = x.Id,
                         PlecarePanaLa = x.PlecarePanaLa,
                         PlecareCuOra = x.PlecareCuOra,
                         TimeString = x.Time.HasValue ? x.Time.Value.Date.ToShortDateString() : "",
                         DataPlecareString = x.DataPlecare.HasValue?x.DataPlecare.Value.ToShortDateString():"",
                         SearchedLocations =
                             (from y in x.SearchedLocations
                                 select new SearchedHistoryLocations {NumeStatie = y.NumeStatie,Type = y.Type,SencondsToWait = TimeSpan.FromSeconds(y.SencondsToWait).TotalMinutes.ToString(CultureInfo.InvariantCulture)}).ToList()
                     }).ToList();
             return View(searchedResults);
        }

         public ActionResult Settings(bool? saved)
         {
             var customerDto = _customerServices.GetCustomer(Identity.Identity.Current.Customer.Id);
             var customerViewModel=new Account
             {
                 LastName = customerDto.LastName,Email = customerDto.Email,FirstName = customerDto.FirstName
             };
             ViewBag.Saved = saved.HasValue && saved.Value;
             return View(customerViewModel);
             
         }

         [HttpPost]
         public ActionResult Settings(Account account)
         {
             if (ModelState.IsValid&&account.Password==account.Password1)
             {          
                 _customerServices.UpdateCustomer(new CustomerDto{Id = Identity.Identity.Current.Customer.Id,Email = account.Email,FirstName = account.FirstName,LastName = account.LastName,NewPassword = account.Password});
                 ViewBag.Saved = true;
             }
             ViewBag.Saved = false;
             return View(account);
         }
  
    }
}