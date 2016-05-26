using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MoreLinq;
using Services.CustomersService;
using Services.CustomerService;
using Services.HistoryDataManagement;
using Services.RouteFinder;
using Services.RouteFinder.Dto;
using TroW.Models;
using TroW.Identity;

namespace TroW.Controllers
{

    public class HomeController : Controller
    {
        private readonly ICustomerServices _customerServices;
        private readonly IRouteFinderService _routeFinderService;
        private readonly IHistoryDataManagementService _dataManagementService;

        public HomeController(ICustomerServices customerServices, IRouteFinderService routeFinderService, IHistoryDataManagementService dataManagementService)
        {
            _customerServices = customerServices;
            _dataManagementService = dataManagementService;
            _routeFinderService = routeFinderService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Contact(bool? success)
        {
            ViewBag.Success = success.HasValue && success.Value;
            return View();
        }
        [HttpPost]
        public ActionResult Contact(ContactViewModel contact)
        {
            var body = new StringBuilder();
            body.AppendLine("Nume: " + contact.FirstName + contact.LastName);
            body.AppendLine("Email: " + contact.Email);
            body.AppendLine("Telefon: " + contact.Phone);
            body.AppendLine(contact.Message);
            _customerServices.SendEmail("trowOffice@gmail.com", body.ToString());

            return RedirectToAction("Contact", new { success = true });
        }


        public ActionResult _NewsLettersPartial()
        {
            var news =
                (from x in _dataManagementService.GetAllNews()
                 select new NewsLetterViewModel { Header = x.Header, Message = x.Message }).ToList();
            return PartialView(news);
        }


        [HttpPost]
        public ActionResult Route(List<Destinations> destinations, string startTime, string endTime, DateTime? leavingDate, bool? reverse)
        {
            var searchedInput = SetRouteInput(destinations, startTime, endTime, leavingDate, reverse);
            if (Identity.Identity.Current.Customer != null)
            {
                _dataManagementService.SaveUserSearch(searchedInput);
            }
            else

                if (reverse.HasValue && reverse.Value)
                    searchedInput.StationToGoThrow.Reverse();
            searchedInput.YearIdentifier = DateTime.Now.Year.ToString();
            var resultDto = _routeFinderService.FindRoute(searchedInput);
            var route = ConvertResultToViewModel(resultDto);
            var allPoints = route.Routes.SelectMany(x => x.Points).ToList();
            var stationToGoThrowCodes = searchedInput.StationToGoThrow.Select(x => x.StationCode).ToList();
            route.StopOverStations = string.Join(",", allPoints.Where(x => stationToGoThrowCodes.Contains(x.CodStatie)).Select(x => x.NumeStatie).ToList());
            route.Input = searchedInput;

            return View(route);
        }

        private RouteFinderInputDto SetRouteInput(List<Destinations> destinations, string startTime, string endTime,
            DateTime? leavingDate, bool? reverse)
        {
            var searchedInput = new RouteFinderInputDto();
            searchedInput.DataPlecare = leavingDate.HasValue ? leavingDate.Value.Date : DateTime.Now;
            searchedInput.OraPlecare = new TimeSpan(0, Int32.Parse(startTime.Split(':')[0]), Int32.Parse(startTime.Split(':')[1]));
            searchedInput.OraSosire = new TimeSpan(0, Int32.Parse(endTime.Split(':')[0]), Int32.Parse(endTime.Split(':')[1]));
            searchedInput.StationToGoThrow = (from x in
                                                  destinations.Where(x => x.Optiune.Trim() == "Via")
                                              select new ViaPlaningDto { SencondsToWait = x.MinuteAsteptare * 60, StationCode = x.CodStatie }).ToList();
            searchedInput.StationToSkip =
    destinations.Where(x => x.Optiune.Trim() == "Exclude").Select(x => x.CodStatie).ToList();
            if (Identity.Identity.Current.Customer != null)
            {
                searchedInput.UserId = Identity.Identity.Current.Customer.Id;
            }
            return searchedInput;

        }

        public ActionResult Route(Guid? id)
        {
            if (id.HasValue)
            {
                var settings = _dataManagementService.GetUserSearch(id.Value);
                settings.YearIdentifier = DateTime.Now.Year.ToString();
                var resultDto = _routeFinderService.FindRoute(settings);
                var route = ConvertResultToViewModel(resultDto);
                route.Input = settings;
                var allPoints = route.Routes.SelectMany(x => x.Points).ToList();
                var stationToGoThrowCodes = settings.StationToGoThrow.Select(x => x.StationCode).ToList();
                route.StopOverStations = string.Join(",", allPoints.Where(x => stationToGoThrowCodes.Contains(x.CodStatie)).Select(x => x.NumeStatie).ToList());
                return View(route);
            }
            return RedirectToAction("Index");
        }
        public ActionResult Login(Guid? guid)
        {
            ViewBag.Guid = guid;
            return View();
        }

        public ActionResult LogOut()
        {
            Identity.Identity.Current.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Login(Account account)
        {
            if (account.IsFacebook)
            {
                if (account.Register)
                {
                    _customerServices.RegisterCustomerFacebook(account.FirstName, account.IdFacebook);
                    return Json(new { success = true });
                }

                var customer = _customerServices.Autentification(account.IdFacebook);
                if (customer != null)
                {
                    Identity.Identity.CreateCustomerIdentity(customer);
                    return Json(new { success = true });
                }

            }
            if (account.ForgatEmail)
            {
                return Json(new { success = _customerServices.SendForgatPasswordEmail(account.Email) });
            }
            if (account.ResetPassword)
            {
                if (account.Password1 == account.Password)
                    return
                        Json(
                            new
                            {
                                success = _customerServices.SetNewPassword(account.Password, account.ForgatPasswordGuid)
                            });
            }
            var customerDto = new CustomerDto
            {
                Email = account.Email,
                Password = account.Password,
                FirstName = account.FirstName,
                LastName = account.LastName
            };
            if (account.Register)
            {
                if (_customerServices.RegisterCustomer(customerDto))
                    return Json(new { success = true });
            }
            if (account.Login)
            {
                var customer = _customerServices.Autentification(customerDto);
                if (customer != null)
                {
                    Identity.Identity.CreateCustomerIdentity(customer);
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false });
        }

        private AllRoutesResults ConvertResultToViewModel(AllRoutesResultsDto reusltDto)
        {
            var routeOutput = new AllRoutesResults();
            routeOutput.Routes = new List<Route>();
            foreach (var route in reusltDto.Routes)
            {
                var newRoute = new Route();

                newRoute.Points = new List<Point>();
                route.Points.ForEach(p =>
                {
                    newRoute.Points.Add(new Point
                    {
                        NumeStatie = p.StatieNume,
                        CodStatie = p.CodStatie,
                        LatLong = p.Position,
                        OraAjungere = TimeSpan.FromSeconds(p.OraAjungere).ToString(@"hh\:mm\:ss")
                    });
                    newRoute.TotalTime =
                        TimeSpan.FromSeconds(route.Points.Last().OraAjungere - route.Points.First().OraAjungere)
                            .ToString(@"hh\:mm\:ss");
                });


                routeOutput.Routes.Add(newRoute);
            }
            return routeOutput;
        }

        public JsonResult GetStationByName(string term)
        {
            var stations =
              _routeFinderService.GetStationThatHaveNameWithTerm(term.ToLower())
                .Select(x => new KeyValuePair<int, string>(x.CodStatie, x.NumeStatie));
            return Json(stations, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Statistics(List<Destinations> destinations, string startTime, string endTime, DateTime? leavingDate, bool? reverse)
        {
            var searchedInput = SetRouteInput(destinations, startTime, endTime, leavingDate, reverse);
            var statistiscDto = _dataManagementService.GetStatistics(searchedInput);
            var statistics = (from x in statistiscDto
                              select new Statistics
                              {
                                  YearIdentifier = x.YearIdentifier,
                                  TotalDuration = (from t in x.TotalDuration select new RouteDurationViewModel { Duration = TimeSpan.FromSeconds(t.Duration).ToString(@"hh\:mm\:ss") }).ToList()
                              }).ToList();

            var allRoutesDetails = new List<RoutePartialDurationViewModel>();
            var allyearsIdentifier = statistiscDto.Select(x => x.YearIdentifier).ToList();
            statistiscDto.ForEach(yearStat =>
            {
                yearStat.TotalPartialDuration.ForEach(y =>
                {
                    allRoutesDetails.Add(new RoutePartialDurationViewModel
                    {
                        StatieDestinatie = y.StatieDestinatie,
                        StatiePlecare = y.StatiePlecare,
                        Duration = TimeSpan.FromSeconds(y.Duration).ToString(@"hh\:mm\:ss"),
                        YearIdentifier = yearStat.YearIdentifier
                    });
                });
            });
            
            var routesDetailsViewModel = new List<RoutePartialDurationViewModel>();
            var routes = allRoutesDetails.GroupBy(x => new { x.StatieDestinatie, x.StatiePlecare }).ToList();
            foreach (var rout in routes)
            {
                var allRout = rout.ToList();
                var currentRoutes =
                    allRout.GroupBy(x => x.YearIdentifier).Where(g => g.Count() == 1).Select(g => g.First()).ToList();
                var newRouteToBeAdded = new RoutePartialDurationViewModel
                {
                    StatieDestinatie = rout.Key.StatieDestinatie,
                    StatiePlecare = rout.Key.StatiePlecare
                };
                var durations = new List<string>();
                allyearsIdentifier.ForEach(year =>
                {
                    var currentYearValue = currentRoutes.SingleOrDefault(x => x.YearIdentifier == year);
                    durations.Add(currentYearValue != null ? currentYearValue.Duration : "");
                });
                newRouteToBeAdded.YearResultDuration = durations;
                routesDetailsViewModel.Add(newRouteToBeAdded);
            }
            ViewBag.RouteDetails = routesDetailsViewModel;
            return PartialView("Statistics", statistics);
        }
    }
}
