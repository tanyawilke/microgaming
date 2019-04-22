using FinanceRequest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinanceRequest.Controllers
{
    public class FileController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: File
        [AllowAnonymous]
        public ActionResult Index(int id)
        {
            var retrieveFile = db.Attachment.FirstOrDefault(c => c.RequestId == id);

            // return File(retrieveFile.Content, retrieveFile.ContentType);
            return ViewBag;
        }
    }
}