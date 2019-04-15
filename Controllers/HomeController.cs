using System;
using System.Data;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Text;
using System.Web.Routing;
using System.Globalization;
using System.Threading;
using System.Data.Entity.Core;
using FinanceRequest.Helpers;
using FinanceRequest.Models;
using System.Collections.Generic;

namespace FinanceRequest.Controllers
{
    public class HomeController : Controller
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

        public ActionResult Index()
        {
            return View();
        }

        public int GetRequestId(string code)
        {
            int myRequest = Convert.ToInt32(db.Request.Where(c => c.ConfirmationCode == code).Select(c => c.Id).Single());

            return myRequest;
        }

        // GET: /Account/ConfirmEmail
        public async Task<ActionResult> ConfirmRequest(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? nameof(ConfirmRequest) : "Error");
        }

        private string SendEmailConfirmationTokenAsync(string userID, int requestId, string subject)
        {
            string code = UserManager.GenerateEmailConfirmationToken(userID);
            var callbackUrl = Url.Action(nameof(ConfirmRequest), "RequestModel",
               new { ad = requestId, userId = userID, code = code }, protocol: Request.Url.Scheme);
            UserManager.SendEmail(userID, subject,
               "Please confirm your ad by clicking <a href=\"" + callbackUrl + "\">here</a>");

            return callbackUrl;
        }

        //[HttpPost]
        //public ActionResult Index([Bind(Include = "Id")] FormCollection requestForm, HttpPostedFileBase[] files)
        //{
        //    TempData["notice"] = "File(s) saved.";
        //    bool fileIsTooLarge = false;
        //    bool fileExtensionInvalid = false;

        //    //Ensure model state is valid  
        //    if (ModelState.IsValid)
        //    {   //iterating through multiple file collection   
        //        foreach (HttpPostedFileBase file in files)
        //        {
        //            //Checking file is available to save.  
        //            if (file != null)
        //            {
        //                var checkextension = new[] { Path.GetExtension(file.FileName).ToLower() };

        //                var allowedFileExtentions = new AllowedFileExtensionsHelper();
        //                var maximumAttachmentSize = new AllowedFileSizeHelper();

        //                // Todo one entrypoint into size check

        //                if (!allowedFileExtentions.FileExtentionAllowed(checkextension))
        //                {
        //                    fileExtensionInvalid = true;
        //                    TempData["notice"] = "Only PDF documents and images (.jpg | .jpeg | .png) may be uploaded.";
        //                }

        //                if (maximumAttachmentSize.AllowedFileSize(files.Count(), file.ContentLength))
        //                {
        //                    fileIsTooLarge = true;
        //                    TempData["notice"] = "A single attachment cannot exceed than 3MB and the total attachment size cannot exceed 15MB.";
        //                }

        //                if ((!fileExtensionInvalid) && (!fileIsTooLarge))
        //                {
        //                    try
        //                    {
        //                        var InputFileName = Path.GetFileName(file.FileName);
        //                        var ServerSavePath = Path.Combine(Server.MapPath("/UploadedFiles") + InputFileName);

        //                        //Save file to server folder  
        //                        file.SaveAs(ServerSavePath);
        //                        //assigning file uploaded status to ViewBag for showing message to user.  
        //                        // ViewBag.UploadStatus = files.Count().ToString() + " file(s) uploaded.";
        //                        TempData["notice"] = files.Count().ToString() + " file(s) uploaded.";
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        TempData["notice"] = ex.Message.ToString();
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return View();
        //}

        //[Authorize]
        //// POST: AdsViewAdverts/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "Id")] FormCollection requestForm, HttpPostedFileBase[] upload)
        {
            var authUser = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                RequestModel request = new RequestModel();
                request.Title = requestForm["Title"];
                request.Description = requestForm["Description"];
                request.Charity = requestForm["Charity"];
                request.PlayItForward = requestForm["PlayItForward"] != null ? true : false;
                request.Amount = Convert.ToDecimal(requestForm["Amount"].Replace(".", ","));
                request.SubmissionDate = DateTime.Today;
                request.ModifyDate = DateTime.Today;
                //request.User = authUser;
                request.Status_Id = 1;
                request.ConfirmationCode = Guid.NewGuid().ToString().Substring(0, 9);

                db.Request.Add(request);

                try
                {
                    db.SaveChanges();

                    int requestId = GetRequestId(request.ConfirmationCode.ToString());

                    if (requestId != 0)
                    {
                        AttachmentModel attachment = new AttachmentModel();

                        TempData["notice"] = "File(s) saved.";
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

                                // Todo one entrypoint into size check

                                if (!allowedFileExtentions.FileExtentionAllowed(checkextension))
                                {
                                    fileExtensionInvalid = true;
                                    TempData["notice"] = "Only PDF documents and images (.jpg | .jpeg | .png) may be uploaded.";
                                }

                                if (maximumAttachmentSize.AllowedFileSize(upload.Count(), file.ContentLength))
                                {
                                    fileIsTooLarge = true;
                                    TempData["notice"] = "A single attachment cannot exceed than 3MB and the total attachment size cannot exceed 15MB.";
                                }

                                if ((!fileExtensionInvalid) && (!fileIsTooLarge))
                                {
                                    try
                                    {
                                        attachment.RequestId = requestId;
                                        attachment.File = Path.GetFileName(file.FileName);

                                        //var InputFileName = Path.GetFileName(file.FileName);
                                        //var ServerSavePath = Path.Combine(Server.MapPath("/UploadedFiles") + InputFileName);

                                        //Save file to server folder  
                                        //file.SaveAs(ServerSavePath);
                                        //assigning file uploaded status to ViewBag for showing message to user.  
                                        // ViewBag.UploadStatus = files.Count().ToString() + " file(s) uploaded.";
                                        TempData["notice"] = upload.Count().ToString() + " file(s) uploaded.";
                                    }
                                    catch (Exception ex)
                                    {
                                        TempData["notice"] = ex.Message.ToString();
                                    }
                                }
                            }
                        }

                        db.SaveChanges();

                        //string callbackUrl = SendEmailConfirmationTokenAsync(authUser, requestId, "Confirm your request.");

                        //return RedirectToAction(nameof(CreateConfirmation), new { uploadedFile = uploadedFileFailure, contactInformation = contactInformationFailure });
                        //return RedirectToAction(nameof(CreateConfirmation), new RouteValueDictionary( new { controller = this, action = "CreateConfirmation", uploadedFile = uploadedFileFailure, contactInformation = contactInformationFailure }));
                    }
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
            }

            return RedirectToAction(nameof(Index));
        }
    }
}