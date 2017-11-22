using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVCManukauTech.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class MembershipTypeViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class MemberShipViewModel
    {
        public MemberShipViewModel()
        {
            this.MembershipType = new List<MembershipTypeViewModel>
            {
                new MembershipTypeViewModel {Id = "MemberAssociate", Name = "Membership Lite (8% discount)"},
                new MembershipTypeViewModel { Id = "MemberFull", Name = "Membership Full (12% discount)" },
                new MembershipTypeViewModel { Id = "NonMember", Name = "Non Member" },
            };
        }

        public bool Success { get; set; }

        [Required(ErrorMessage = "Must Select Membership")]
        public List<MembershipTypeViewModel> MembershipType { get; set; }

        [Display(Name = "Membership Type")]

        [Required(ErrorMessage = "Must Select Membership")]
        public string SelectedMembershipType { get; set; }

        [Required(ErrorMessage = "Card owner is required")]
        public string CardOwner { get; set; }
        [Required(ErrorMessage = "Card type is required")]
        public string CardType { get; set; }
        [Required(ErrorMessage = "Card number is required")]
        public string CardNumber { get; set; }
        [Required(ErrorMessage = "Card CSC is required")]
        public string CSC { get; set; }
    }

    public class RegisterViewModel
    {
        //public RegisterViewModel()
        //{
        //    this.MembershipType = new List<MembershipTypeViewModel>
        //    {
        //        new MembershipTypeViewModel {Id = "MemberAssociate", Name = "Membership Lite (8% discount)"},
        //        new MembershipTypeViewModel { Id = "MemberFull", Name = "Membership Full (12% discount)" },
        //        new MembershipTypeViewModel { Id = "NonMember", Name = "Non Member" },
        //    };
        //}

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        //public List<MembershipTypeViewModel> MembershipType { get; set; }

        //[Display(Name = "Membership Type")]
        //public string SelectedMembershipType { get; set; }

        //[Required(ErrorMessage = "Card owner is required")]
        //public string CardOwner { get; set; }
        //[Required(ErrorMessage = "Card type is required")]
        //public string CardType { get; set; }
        //[Required(ErrorMessage = "Card number is required")]
        //public string CardNumber { get; set; }
        //[Required(ErrorMessage = "Card CSC is required")]
        //public string CSC { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}