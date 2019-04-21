using FinanceRequest.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
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

        // GET: AdsViewAdverts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BigViewModel bigViewModel = new BigViewModel();
            bigViewModel.Request = db.Request.Find(id);
            bigViewModel.Attachment = db.Attachment.SingleOrDefault(c => c.RequestId == id);
            // bigViewModel.Status = db.Status.SingleOrDefault(c => c.RequestId == id);

            return View(bigViewModel);
        }

        // POST: RequestList/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind(Include = "Id")] FormCollection requestForm, HttpPostedFileBase upload)
        {
            bool contactInformationFailure = false;
            bool uploadedFileFailure = false;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var authUser = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                //AdsViewAdverts advert = db.AdsViewAdverts.Find(id);
                //AdsViewContact contact = db.AdsViewContacts.FirstOrDefault(c => c.Advert.Id == id);
                //AdsViewFile attachment = db.AdsViewFile.FirstOrDefault(c => c.AdId == id);

                //advert.Title = adsViewAdverts["AdsViewAdverts.Title"];
                //advert.Description = adsViewAdverts["AdsViewAdverts.Description"];
                //advert.DateLastModified = thisDay;
                //DateTime publishDate = DateTime.ParseExact(adsViewAdverts["AdsViewAdverts.DatePublished"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //DateTime dateExpiry = DateTime.ParseExact(adsViewAdverts["AdsViewAdverts.ExpiryDate"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                ////string publishDate = adsViewAdverts["DatePublished"].ToString().;
                //advert.DatePublished = Convert.ToDateTime(publishDate);
                //////advert.DatePublished = thisDay;
                //advert.ExpiryDate = Convert.ToDateTime(dateExpiry);

                //advert.Price = Convert.ToDecimal(adsViewAdverts["AdsViewAdverts.Price"].Replace(".", ","));
                ////advert.DateAdvertAdded = thisDay;
                ////advert.DateOfDeleteAdvert = Convert.ToDateTime(dateExpiry);
                //advert.CategoryId = Int32.Parse(adsViewAdverts["AdsViewAdverts.CategoryId"]);
                //advert.AdvertTypeId = Int32.Parse(adsViewAdverts["AdsViewAdverts.AdvertTypeId"]);
                //advert.LocationId = Int32.Parse(adsViewAdverts["AdsViewAdverts.LocationId"]);
                //advert.SubCategoryId = Int32.Parse(adsViewAdverts["AdsViewAdverts.SubCategoryId"]);
                //advert.User = authUser;

                //contact.ContactName = adsViewAdverts["AdsViewContact.ContactName"];
                //contact.ContactEmail = adsViewAdverts["AdsViewContact.ContactEmail"];
                //contact.ContactPhoneNumber = adsViewAdverts["AdsViewContact.ContactPhoneNumber"];
                //contact.ContactPhysicalAddress = adsViewAdverts["AdsViewContact.ContactPhysicalAddress"];
                //contact.WebSite = adsViewAdverts["AdsViewContact.WebSite"];

                //db.Entry(advert).State = EntityState.Modified;
                //db.Entry(contact).State = EntityState.Modified;

                //var addFile = false;


                //if (attachment == null)
                //{
                //    addFile = true;
                //}

                //if (upload != null && upload.ContentLength > 0)
                //{
                //    if (addFile)
                //    {
                //        AdsViewFile newAttachment = new AdsViewFile();
                //        newAttachment.AdId = advert.Id;
                //        newAttachment.Filename = Path.GetFileName(upload.FileName);
                //        newAttachment.FileType = FileType.Avatar;
                //        newAttachment.ContentType = upload.ContentType;

                //        attachment = newAttachment;
                //    }
                //    else
                //    {
                //        attachment.AdId = advert.Id;
                //        attachment.Filename = Path.GetFileName(upload.FileName);
                //        attachment.FileType = FileType.Avatar;
                //        attachment.ContentType = upload.ContentType;
                //    }


                //    string[] allowedExtentions = GetAllowedExtension();

                //    foreach (var extention in allowedExtentions)
                //    {
                //        if (attachment.ContentType.Contains(extention.ToLower()))
                //        {
                //            try
                //            {
                //                using (var reader = new BinaryReader(upload.InputStream))
                //                {
                //                    attachment.Content = reader.ReadBytes(upload.ContentLength);

                //                    if (addFile)
                //                    {
                //                        db.Attachment.Add(attachment);
                //                    }
                //                    else
                //                    {

                //                        db.Entry(attachment).State = EntityState.Modified;
                //                    }

                //                }

                //                break;
                //            }
                //            catch (Exception ex)
                //            {
                //                uploadedFileFailure = true;
                //            }
                //        }
                //    }
                //}

                try
                {
                    db.SaveChanges();
                    return RedirectToAction(nameof(MyRequests));
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            ViewBag.Message("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                        }
                    }
                }
                catch (OptimisticConcurrencyException)
                {
                    db.SaveChanges();
                }
            }

            return View();
        }
    }
}