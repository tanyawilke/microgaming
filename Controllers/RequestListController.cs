using FinanceRequest.Helpers;
using FinanceRequest.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FinanceRequest.Controllers
{
    public class RequestListController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: RequestList
        [Authorize(Roles = "Admin")]
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

            BigViewModel bigViewModel = new BigViewModel();
            bigViewModel.Request = db.Request.Find(id);
            bigViewModel.Attachment = db.Attachment.FirstOrDefault(c => c.RequestId == id);
            bigViewModel.Status = db.Status.SingleOrDefault(c => c.Id == id);

            return View(bigViewModel);
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

            if (User.IsInRole("Admin"))
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(MyRequests));
        }

        // GET: RequestList/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BigViewModel bigViewModel = new BigViewModel();
            bigViewModel.Request = db.Request.Find(id);
            bigViewModel.Attachment = db.Attachment.FirstOrDefault(c => c.RequestId == id);

            ViewBag.StatusList = db.Status;

            return View(bigViewModel);
        }

        // POST: RequestList/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind(Include = "Id")] FormCollection requestForm, HttpPostedFileBase[] upload)
        {
            bool uploadedFileFailure = false;
            string uploadedFileMessage = string.Empty;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var authUser = User.Identity.GetUserId();


            if (ModelState.IsValid)
            {
                var request = db.Request.Find(id);
                var attachment = db.Attachment.SingleOrDefault(c => c.Id == id);
                
                request.Title = requestForm["Request.Title"];
                request.Description = requestForm["Request.Description"];
                request.ModifyDate = DateTime.Today;
                request.Amount = Convert.ToDecimal(requestForm["Request.Amount"]);
                request.StatusId = requestForm["Request.StatusId"] != null ? Int32.Parse(requestForm["Request.StatusId"]) : 1;

                db.Entry(request).State = EntityState.Modified;

                var addFile = false;

                if (attachment == null)
                {
                    addFile = true;
                }

                bool fileIsTooLarge = false;
                bool fileExtensionInvalid = false;

                foreach (HttpPostedFileBase file in upload)
                {
                    //Checking file is available to save.  
                    if (file != null)
                    {
                        var checkextension = new[] { Path.GetExtension(file.FileName).ToLower() };

                        var allowedFileExtentions = new AllowedFileExtensionsHelper();
                        var maximumAttachmentSize = new AllowedFileSizeHelper();

                        if (!allowedFileExtentions.FileExtentionAllowed(checkextension))
                        {
                            fileExtensionInvalid = true;
                            uploadedFileMessage = "Only PDF documents and images (.jpg | .jpeg | .png) may be uploaded.";
                            uploadedFileFailure = true;
                        }

                        if (maximumAttachmentSize.AllowedFileSize(upload.Count(), file.ContentLength))
                        {
                            fileIsTooLarge = true;
                            uploadedFileMessage = "A single attachment cannot exceed than 3MB and the total attachment size cannot exceed 15MB.";
                            uploadedFileFailure = true;
                        }

                        if ((!fileExtensionInvalid) && (!fileIsTooLarge))
                        {
                            try
                            {
                                attachment.RequestId = id;
                                attachment.File = Path.GetFileName(file.FileName);
                                attachment.ContentType = file.ContentType;

                                using (var reader = new BinaryReader(file.InputStream))
                                {
                                    attachment.Content = reader.ReadBytes(file.ContentLength);

                                    db.Attachment.Add(attachment);
                                }
                            }
                            catch (Exception ex)
                            {
                                uploadedFileMessage = ex.Message.ToString();
                                uploadedFileFailure = true;
                            }
                        }
                    }
                }

                try
                {
                    db.SaveChanges();
                    return RedirectToAction(nameof(EditConfirmation), new { message = uploadedFileMessage, uploadedFile = uploadedFileFailure });
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

        public ActionResult EditConfirmation(string fileMessage, bool uploadedFile = false)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("Your request has been updated successfully.");

            if (uploadedFile)
            {
                message.AppendLine("Note that an error occurred when you attempted to upload your supporting documentation.  " + fileMessage + " Please edit your request and upload the file in its correct format.");
            }
            else
            {
                message.AppendLine(fileMessage);
            }

            ViewBag.Message = message;

            return View();
        }
    }
}