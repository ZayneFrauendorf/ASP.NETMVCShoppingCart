using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCManukauTech.Models
{

    public class ContentViewModel
    {
        public string HomeHeadingTitle { get; set; }
        [Required(ErrorMessage = "Heading title content is required")]
        public string HomeHeadingData { get; set; }
        public string HomeText1Title { get; set; }
        [Required(ErrorMessage = "Home text 1 content is required")]
        public string HomeText1Data { get; set; }
        public string CopyrightNoteTitle { get; set; }
        [Required(ErrorMessage = "Copyright note text content is required")]
        public string CopyrightNoteData { get; set; }
    }
}