using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using MVCManukauTech.Models;
using MVCManukauTech.Services;
using Newtonsoft.Json;

namespace MVCManukauTech.Controllers
{
    public class HomeController : Controller
    {
        private XSpy2Entities db = new XSpy2Entities();

        public async Task<ActionResult> Index()
        {
            var contentData = await db.SiteContents.ToListAsync();
            var contentViewModel = new ContentViewModel();
            foreach (var dbItem in contentData)
            {
                if (dbItem.Id == 1)
                {
                    contentViewModel.HomeHeadingTitle = dbItem.ContentDesc;
                    contentViewModel.HomeHeadingData = dbItem.ContentText;
                }

                if (dbItem.Id == 2)
                {
                    contentViewModel.HomeText1Title = dbItem.ContentDesc;
                    contentViewModel.HomeText1Data = dbItem.ContentText;
                }

                if (dbItem.Id == 3)
                {
                    contentViewModel.CopyrightNoteTitle = dbItem.ContentDesc;
                    contentViewModel.CopyrightNoteData = dbItem.ContentText;
                }

            }

            return View(contentViewModel);
        }

        //[Authorize]
        //public ActionResult About()
        //{
        //    ViewBag.Message = "Everything you need to know...";

        //    return View();
        //}

            public async Task<ActionResult> About()
        {
            ViewBag.Message = "Everything you need to know...";
            var contentData = await db.SiteContents.ToListAsync();
            HomeContactViewModel viewModel = new HomeContactViewModel
            {
                AboutContent = contentData.FirstOrDefault(item => item.Id == AppConstants.AboutContentId).ContentText
            };
            return View(viewModel);
        }

        public async Task<ActionResult> Contact()
        {
            ViewBag.Message = "Anytime, anywhere just...";
            var contentData = await db.SiteContents.ToListAsync();
            HomeContactViewModel viewModel = new HomeContactViewModel
            {
                Address1 = contentData.FirstOrDefault(item => item.Id == AppConstants.Address1Id).ContentText,
                Address2 = contentData.FirstOrDefault(item => item.Id == AppConstants.Address2Id).ContentText,
                Phone = contentData.FirstOrDefault(item => item.Id == AppConstants.PhoneId).ContentText,
                MarketingEmail = contentData.FirstOrDefault(item => item.Id == AppConstants.MarketingEmailId).ContentText,
                SupportEmail = contentData.FirstOrDefault(item => item.Id == AppConstants.SupportEmailId).ContentText
            };

            return View(viewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> About(HomeContactViewModel viewModel, XSpy2Entities db1 = null)
        {
            if(db1 != null)
            {
                this.db = db1;
            }

            try
            {
                var contentData = await db.SiteContents.ToListAsync();
                var aboutContent = contentData.FirstOrDefault(item => item.Id == AppConstants.AboutContentId);
                if(aboutContent != null)
                {
                    aboutContent.ContentText = viewModel.AboutContent;
                }
                var result = await db.SaveChangesAsync();
            }
            catch (System.Exception exception)
            {
                ViewBag.Message = "Things broke!";
                //this.Log.Error(exception);
                return View(viewModel);
            }

            return this.RedirectToAction("about");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Contact(HomeContactViewModel viewModel, XSpy2Entities db1 = null)
        {
            if(db1 != null)
            {
                this.db = db1;
            }

            try
            {
                var contentData = await db.SiteContents.ToListAsync();
                var address1 = contentData.FirstOrDefault(item => item.Id == AppConstants.Address1Id);
                var address2 = contentData.FirstOrDefault(item => item.Id == AppConstants.Address2Id);
                var phone = contentData.FirstOrDefault(item => item.Id == AppConstants.PhoneId);
                var marketingEmail = contentData.FirstOrDefault(item => item.Id == AppConstants.MarketingEmailId);
                var supportEmail = contentData.FirstOrDefault(item => item.Id == AppConstants.SupportEmailId);
                if (address1 != null)
                {
                    address1.ContentText = viewModel.Address1;
                }

                if (address2 != null)
                {
                    address2.ContentText = viewModel.Address2;
                }

                if (phone != null)
                {
                    phone.ContentText = viewModel.Phone;
                }
                if (marketingEmail != null)
                {
                    marketingEmail.ContentText = viewModel.MarketingEmail;
                }

                if (supportEmail != null)
                {
                    supportEmail.ContentText = viewModel.SupportEmail;
                }

                var result = await db.SaveChangesAsync();
            }
            catch (System.Exception exception)
            {
                ViewBag.Message = "Things broke!";
                //this.Log.Error(exception);
                return View(viewModel);
            }

            return this.RedirectToAction("contact");
        }

        public string GetContactContent()
        {
            string SQL = "SELECT * FROM Customers";
            var content = db.Database.SqlQuery<ContactContentViewModel>(SQL);
            string json = JsonConvert.SerializeObject(content);
            // AngularJS needs a JSON "envelope" which is containing code like {records[ .... ]}
            // where [ .... ] represents the JSON we usually get out of the above SerializeObject method
            return "{\"records\":" + json + "}";
        }

    }
}
