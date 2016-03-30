using System.Linq;
using System.Web.Mvc;
using Omu.ValueInjecter;
using SL.Core.Infrastructure;
using SL.Services;
using SL.Web.Infrastructure;
using SL.Web.ViewModels.Customer;

namespace SL.Web.Controllers
{
  [CustomerAuthorize]
  public class MainController : Controller
  {
    private readonly IShootingService shootingService;
    private readonly ICustomerServices customerService;
    private readonly ICommercialsService commercialsService;
    private readonly ICustomerFooterPageService customerFooterPageService;
    public MainController(IShootingService shootingService, ICommercialsService commercialsService, ICustomerServices customerService, ICustomerFooterPageService customerFooterPageService)
    {
      this.shootingService = shootingService;
      this.commercialsService = commercialsService;
      this.customerService = customerService;
      this.customerFooterPageService = customerFooterPageService;
    }
    // GET: Main
    public ActionResult Index()
    {
      var applicationUrl = string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority);
      var mainPageViewModel = new MainPageViewModel
      {
        CustomerShootings = (from x in shootingService.GetCustomerShootings(Identity.Current.Customer.Id, applicationUrl)
                             select (MainPageShootingViewModel)new MainPageShootingViewModel().InjectFrom(x)).ToList(),
        MainPageCommercials = (from x in commercialsService.GetCustomerCommercials(Identity.Current.Customer.Id, 3, applicationUrl)
                               select new MainPageCommercial
                               { 
                                 Id = x.Id,
                                 CommercialButtonText = x.ButtonText,
                                 CommercialHeadline = x.Headline,
                                 CommercialText = x.Text,
                                 CommercialImageUrl = x.ImageUrl,
                                 CommercialButtonLinkAddress = x.ButtonLink,
                                 CommercialVideoUrl = !string.IsNullOrEmpty(x.VideoUrl) && !x.VideoUrl.Contains("autoplay") ? x.VideoUrl + "&autoplay=1" : null
                               }).Take(3).ToList()
      }; 

      mainPageViewModel.CustomerName = Identity.Current.Customer.FirstName;
      mainPageViewModel.CustomerAgreedWithTerms = Identity.Current.Customer.CustomerAgreesWithTerms;
      if (!mainPageViewModel.CustomerAgreedWithTerms)
      {
        var agbPage = customerFooterPageService.GetCustomerFooterPage(1);
        mainPageViewModel.AgbText = agbPage != null ? agbPage.PageContent : string.Empty;  
      }
      return View(mainPageViewModel);
    }

    public ActionResult Gallery(string shootingCode)
    {
      var customerShootingDto = shootingService.GetCustomerShooting(shootingCode, string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority));
      var customerShootingViewModel = (CustomerShootingViewModel)new CustomerShootingViewModel().InjectFrom(customerShootingDto);
      customerShootingViewModel.ShootingCode = shootingCode;
      customerShootingViewModel.PictureList = (from image in customerShootingDto.PictureList select (CustomerPictureViewModel)new CustomerPictureViewModel().InjectFrom(image)).ToList();
      customerShootingViewModel.CustomerShootingInvites = (from x in customerService.GetCustomerShootingShareInvites(Identity.Current.Customer.Id, shootingCode) select (CustomerShootingInvitationViewModel) new CustomerShootingInvitationViewModel().InjectFrom(x)).ToList();
      return View(customerShootingViewModel);
    }

    public JsonResult DownloadPicture(string shootingCode, string fileName)
    {
      var document = shootingService.DownloadCustomerPicture(shootingCode, fileName);
      DownloadHelper.CreateImageFileForUserDownload(Response, document, fileName);
      return Json(null, JsonRequestBehavior.AllowGet);
    }

    [HttpPost]
    public ActionResult InviteFriend(string friendEmail, string shootingCode)
    {
      var customerShootingInviteDto = customerService.SendCustomerShootingShareInvitation(Identity.Current.Customer.Id, shootingCode, friendEmail);
      var customerShootingInviteViewModel = (CustomerShootingInvitationViewModel) new CustomerShootingInvitationViewModel().InjectFrom(customerShootingInviteDto);
      return PartialView("InviteRow", customerShootingInviteViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteShare(int id)
    {
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult AgbAgreed()
    {
      customerService.CustomerAgreesWithTerms(Identity.Current.Customer.Id);
      Identity.CustomerAgreedWithTheTerms();
      return RedirectToAction("Index", "Main");
    }
  }
}