using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using MVCManukauTech.Controllers;
using MVCManukauTech.Models;
using MVCManukauTech.Services;
using Xunit;
using System.Web;

namespace MVCManukauTech.Tests1
{
    public class HomeControllerUnitTests
    {
        [Fact(DisplayName = "ContactPostUT1")]
        public async Task ContactPostUnitTest1()
        {
            // Create some test data
            var data = new List<SiteContent>
            {
                new SiteContent { Id = AppConstants.Address1Id, ContentText = "Some random text", ContentDesc = null }
            };

            // Create a mock set and context
            var set = new Mock<DbSet<SiteContent>>()
                .SetupData(data);

            var context = new Mock<XSpy2Entities>();
            context.Setup(c => c.SiteContents).Returns(set.Object);

            context.Setup(c => c.SaveChangesAsync()).Returns(ReturnFakeInt);

            //// Create a BlogsController and invoke the Index action
            //var controller = new BlogsController(context.Object);
            //var result = await controller.Index();

            //// Check the results
            //var blogs = (List<Blog>)result.Model;
            //Assert.AreEqual(3, blogs.Count());
            //Assert.AreEqual("AAA", blogs[0].Name);
            //Assert.AreEqual("BBB", blogs[1].Name);
            //Assert.AreEqual("CCC", blogs[2].Name);

            var homeController = new HomeController();
            var result = await homeController.Contact(new Models.HomeContactViewModel(), context.Object);
            Assert.IsType<RedirectToRouteResult>(result);
        }


        [Fact(DisplayName = "ContactPostUT2")]
        public async Task ContactPostUnitTest2()
        {
            // Create some test data
            var data = new List<SiteContent>
            {
                new SiteContent { Id = AppConstants.Address1Id, ContentText = "Some random text", ContentDesc = null }
            };

            // Create a mock set and context
            var set = new Mock<DbSet<SiteContent>>()
                .SetupData(data);

            var context = new Mock<XSpy2Entities>();
            context.Setup(c => c.SiteContents).Returns(set.Object);

            context.Setup(c => c.SaveChangesAsync()).Returns(ReturnFakeInt);

            //// Create a BlogsController and invoke the Index action
            //var controller = new BlogsController(context.Object);
            //var result = await controller.Index();

            //// Check the results
            //var blogs = (List<Blog>)result.Model;
            //Assert.AreEqual(3, blogs.Count());
            //Assert.AreEqual("AAA", blogs[0].Name);
            //Assert.AreEqual("BBB", blogs[1].Name);
            //Assert.AreEqual("CCC", blogs[2].Name);

            var homeController = new HomeController();
            var result = await homeController.Contact(new Models.HomeContactViewModel { Address1 = "My new value" }, context.Object);
            // Assert.IsType<RedirectToRouteResult>(result);
            Assert.Equal(data[0].ContentText, "My new value");
        }

        public async Task<int> ReturnFakeInt()
        {
            return 0;
        }

        //[Fact(DisplayName = "Login")]
        //public Task<ActionResult> Login()
        //{
            
        //    var controller = new AccountController();
        //    //Session["IsMembershipExpired"] = "YES";
        //    //            }

        //    //            return RedirectToLocal(returnUrl);
        //    //        case SignInStatus.LockedOut:
        //    //            return View("Lockout");
        //    //        case SignInStatus.RequiresVerification:
        //    //            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
        //    //        case SignInStatus.Failure:
        //    //        default:
        //    //            ModelState.AddModelError("", "Invalid login attempt.");
        //    //            return View(model);
        //    //    }
        
        //}
    }
}