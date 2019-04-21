using FinanceRequest.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace FinanceRequest.Controllers
{
    public class RequestListController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager;
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: RequestList
        public ActionResult Index()
        {
            var allRequests = db.Request;

            if (allRequests != null)
            {
                return View(allRequests.ToList());
            }
            else
            {
                return View("There are no requests.");
            }
        }

        // GET: RequestList/MyRequests/5
        public ActionResult MyRequests(ApplicationUser user)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                var loggedUser = User.Identity.GetUserId();

                try
                {
                    var myRequests = db.Request.Where(c => c.User == loggedUser);

                    if (myRequests != null)
                    {
                        return View(myRequests);
                    }
                    else
                    {
                        return View("You don't have any requests.");
                    }

                }
                catch (Exception Ex)
                {
                    return View(Ex.InnerException.ToString());
                }
            }
        }

        // GET: RequestList/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                var requestDetails = db.Request.Where(c => c.Id == id);

                if (requestDetails != null)
                {
                    return View(requestDetails);
                }
                else
                {
                    return View("You don't have any requests.");
                }

            }
            catch (Exception Ex)
            {
                return View(Ex.InnerException.ToString());
            }

            //BigViewModel bigViewModel = new BigViewModel();
            //bigViewModel.AdsViewAdverts = db.AdsViewAdverts.Find(id);
            //bigViewModel.AdsViewContact = db.AdsViewContacts.FirstOrDefault(c => c.Advert.Id == id);
            //bigViewModel.AdsViewFile = db.AdsViewFile.FirstOrDefault(c => c.AdId == id);

            // return View(bigViewModel);
        }

        // GET: RequestList/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var loggedUser = User.Identity.GetUserId();

            try
            {
                var myRequests = db.Request.Where(c => c.User == loggedUser);

                if (myRequests != null)
                {
                    var request = db.Request.Find(id);

                    if (request == null)
                    {
                        return HttpNotFound();
                    }

                    return View(request);
                }
            }
            catch (Exception ex)
            {

            }

            return View();

        }

        // POST: RequestList/Delete/5
        [HttpPost, ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var attachment = db.Attachment.SingleOrDefault(c => c.Id == id);

            if (attachment != null)
            {
                db.Attachment.Remove(attachment);
            }

            var request = db.Request.Find(id);
            if (request != null)
            {
                db.Request.Remove(request);
            }

            db.SaveChanges();

            return RedirectToAction(nameof(MyRequests));
        }
    }
}