using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCManukauTech.Models
{
    public class ContactContentViewModel
    {
        //Discontinued, original idea but too coupled in the end.
        //public int ContactContentId { get; set; }
        //public string ContactContentText { get; set; }
    }

    public class HomeContactViewModel
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Phone { get; set; }
        public string SupportEmail { get; set; }
        public string MarketingEmail { get; set; }
        public string AboutContent { get; set; }
    }
}