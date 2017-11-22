using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MVCManukauTech.Models;

namespace MVCManukauTech.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ContentAdminController : Controller
    {
    //    private XSpy2Entities db = new XSpy2Entities();

    //    // GET: ContentAdmin
    //    public async Task<ActionResult> Index()
    //    {
    //        var contentData = await db.SiteContents.ToListAsync();
    //        var contentViewModel = new ContentViewModel();
    //        foreach (var dbItem in contentData)
    //        {
    //            if (dbItem.Id == 1)
    //            {
    //                contentViewModel.HomeHeadingTitle = dbItem.ContentDesc;
    //                contentViewModel.HomeHeadingData = dbItem.ContentText;
    //            }

    //            if (dbItem.Id == 2)
    //            {
    //                contentViewModel.HomeText1Title = dbItem.ContentDesc;
    //                contentViewModel.HomeText1Data = dbItem.ContentText;
    //            }

    //            if (dbItem.Id == 3)
    //            {
    //                contentViewModel.CopyrightNoteTitle = dbItem.ContentDesc;
    //                contentViewModel.CopyrightNoteData = dbItem.ContentText;
    //            }
    //        }

    //        return View(contentViewModel);
    //    }

    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public async Task<ActionResult> Index(ContentViewModel contentViewModel)
    //    {
    //        if (!ModelState.IsValid)
    //        {
    //            ModelState.AddModelError("", "Incomplete data, try again");
    //            return View(contentViewModel);
    //        }

    //        var contentData = await db.SiteContents.ToListAsync();
    //        foreach (var dbItem in contentData)
    //        {
    //            if (dbItem.Id == 1)
    //            {
    //                dbItem.ContentDesc = contentViewModel.HomeHeadingTitle;
    //                dbItem.ContentText = contentViewModel.HomeHeadingData;
    //            }

    //            if (dbItem.Id == 2)
    //            {
    //                dbItem.ContentDesc = contentViewModel.HomeText1Title;
    //                dbItem.ContentText = contentViewModel.HomeText1Data;
    //            }

    //            if (dbItem.Id == 3)
    //            {
    //                dbItem.ContentDesc = contentViewModel.CopyrightNoteTitle;
    //                dbItem.ContentText = contentViewModel.CopyrightNoteData;
    //            }
    //        }

    //        await db.SaveChangesAsync();
    //        return RedirectToAction("Index");
    //    }



    }
}