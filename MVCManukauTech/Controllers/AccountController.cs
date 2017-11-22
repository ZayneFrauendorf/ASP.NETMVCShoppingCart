using System.Globalization;
using MVCManukauTech.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Net;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MVCManukauTech.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        private ApplicationSignInManager _signInManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doen't count login failures towards lockout only two factor authentication
            // To enable password failures to trigger lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    var possibleUser = await UserManager.FindByEmailAsync(model.Email);
                    if (null != possibleUser && possibleUser.MemberExpireAt.HasValue && possibleUser.MemberExpireAt.Value <= DateTime.UtcNow)
                    {
                        Session["IsMembershipExpired"] = "YES";
                    }

                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            var user = await UserManager.FindByIdAsync(await SignInManager.GetVerifiedUserIdAsync());
            if (user != null)
            {
                ViewBag.Status = "For DEMO purposes the current " + provider + " code is: " + await UserManager.GenerateTwoFactorTokenAsync(user.Id, provider);
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: false, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        [AllowAnonymous]
        public ActionResult BecomeMember()
        {
            return View(new MemberShipViewModel());
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BecomeMember(MemberShipViewModel model)
        {
            
            if (ModelState.IsValid)
            {           
                if (model.SelectedMembershipType.ToLowerInvariant() != "memberfull" && model.SelectedMembershipType.ToLowerInvariant() != "memberassociate")
                {
                    ModelState.AddModelError("", "Select a membership type first");
                    return View(model);

                    
                }

                if (!await this.RoleManager.RoleExistsAsync(model.SelectedMembershipType))
                {
                    ModelState.AddModelError("", "Invalid membership type");
                    return View(model);
                }

                var membershipFees = "10.00";
                if (model.SelectedMembershipType == "MemberFull")
                {
                    membershipFees = "20.00";
                }

                string expiryDatePlaceHolder = "2018-10-01";
                System.Net.WebClient w = new System.Net.WebClient();
                //POST has some optional configurations - recommended
                w.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                w.Encoding = System.Text.Encoding.UTF8;

                string webAddress = "https://island.manukau.ac.nz/BankFiction2/Transactions/Reservation";
                string data = "MerchantId=Kim@a.a&MerchantPassword=nice.coffee&CardNo={0}&CardType={1}&CardSecurity={2}&CardHolder={3}&CardExpiry={4}&Amount={5}";
                string cardOwner = HttpUtility.UrlEncode(model.CardOwner);

                data = String.Format(data, model.CardNumber, model.CardType, model.CSC, cardOwner, expiryDatePlaceHolder, membershipFees);

                //POST - another difference from GET is method UploadString 
                string responseJson = w.UploadString(webAddress, data);

                //To work with JSON we add a "using" statement at the top of this document -- using Newtonsoft.Json;
                Reservation reservation = JsonConvert.DeserializeObject<Reservation>(responseJson);

                if (!reservation.IsReserved)
                {
                    ModelState.AddModelError("", "Payment with given card details failed, try again!");
                    return View(model);
                }

                ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());

                try
                {
                    var roles = await UserManager.GetRolesAsync(user.Id);
                    await UserManager.RemoveFromRolesAsync(user.Id, roles.ToArray());
                    await UserManager.UpdateAsync(user);
                    this.UserManager.AddToRole(user.Id, model.SelectedMembershipType);
                    user.MembershipPayId = reservation.TransactionId.ToString();
                    user.MemberExpireAt = DateTime.UtcNow.AddYears(1);
                    await UserManager.UpdateAsync(user);
                    model.Success = true;
                    var authenticationManager = HttpContext.GetOwinContext().Authentication;                    authenticationManager.SignOut();

                    var identity = UserManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                    authenticationManager.SignIn(new Microsoft.Owin.Security.AuthenticationProperties() { IsPersistent = true }, identity);


                }
                catch (Exception exception)
                {
                    ViewBag.Message = "Things broke!";
                    Debug.WriteLine(exception);
                    //this.Log.Error(exception);
                    return View(model);
                }
            }
            // If we got this far, something failed, redisplay form

            return View("Success");

        }

        public ActionResult Success()
        {
            return View();
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
                if (ModelState.IsValid)
                {
                    //140813 JPC check Captcha validity.
                    if (!CaptchaMvc.HtmlHelpers.CaptchaHelper.IsCaptchaValid(this, "Please try again with the 'Enter the text you see ...' check below."))
                    {
                        //Captcha input does not match.  Replay form for user to try again

                        //Add lots of error messages just to show that we can
                        ModelState.AddModelError("", "Error Message 01");
                        ModelState.AddModelError("", "Error Message 02");
                        ModelState.AddModelError("", "Error Message 03");
                        ModelState.AddModelError("", "Error Message 04");
                        return View(model);
                    }

                    //if (model.SelectedMembershipType.ToLowerInvariant() != "memberfull" && model.SelectedMembershipType.ToLowerInvariant() != "memberassociate")
                    //{
                    //    ModelState.AddModelError("", "Select a membership type first");
                    //    return View(model);
                    //}

                    //if (!await this.RoleManager.RoleExistsAsync(model.SelectedMembershipType))
                    //{
                    //    ModelState.AddModelError("", "Invalid membership type");
                    //    return View(model);
                    //}

                    //var membershipFees = "10.00";
                    //if (model.SelectedMembershipType == "MemberFull")
                    //{
                    //    membershipFees = "20.00";
                    //}

                    //string expiryDatePlaceHolder = "2018-10-01";
                    //System.Net.WebClient w = new System.Net.WebClient();
                    ////POST has some optional configurations - recommended
                    //w.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    //w.Encoding = System.Text.Encoding.UTF8;

                    //string webAddress = "https://island.manukau.ac.nz/BankFiction2/Transactions/Reservation";
                    //string data = "MerchantId=Kim@a.a&MerchantPassword=nice.coffee&CardNo={0}&CardType={1}&CardSecurity={2}&CardHolder={3}&CardExpiry={4}&Amount={5}";
                    //string cardOwner = HttpUtility.UrlEncode(model.CardOwner);

                    //data = String.Format(data, model.CardNumber, model.CardType, model.CSC, cardOwner, expiryDatePlaceHolder, membershipFees);

                    ////POST - another difference from GET is method UploadString 
                    //string responseJson = w.UploadString(webAddress, data);

                    ////To work with JSON we add a "using" statement at the top of this document -- using Newtonsoft.Json;
                    //Reservation reservation = JsonConvert.DeserializeObject<Reservation>(responseJson);

                    //if (!reservation.IsReserved)
                    //{
                    //    ModelState.AddModelError("", "Payment with given card details failed, try again!");
                    //    return View(model);
                    //}
                try
                {
                
                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email, EmailConfirmed = true/*, MembershipPayId = reservation.TransactionId.ToString(), MemberExpireAt = DateTime.UtcNow.AddYears(1)*/ };
                    var result = await UserManager.CreateAsync(user, model.Password);
                    
                    if (result.Succeeded)
                    {
                        this.UserManager.AddToRole(user.Id, "NonMember");
                        //this.UserManager.AddToRole(user.Id, model.SelectedMembershipType);
                        var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                        ViewBag.Link = callbackUrl;
                        if (user.MemberExpireAt.HasValue && user.MemberExpireAt.Value <= DateTime.UtcNow)
                        {
                            Session["IsMembershipExpired"] = "YES";
                        }

                        return View("DisplayEmail");
                    }
                    AddErrors(result);
                } catch(Exception e)
                {
                    Debug.WriteLine(e);
                }
                }

            // If we got this far, something failed, redisplay form

                return View(model);        
    }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
                ViewBag.Link = callbackUrl;
                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await this.UserManager.AddToRoleAsync(user.Id, "NonMember");
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
       
    }
}