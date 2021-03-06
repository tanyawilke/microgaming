﻿using System;
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

        [Authorize]
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

        [Authorize]
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "Id")] FormCollection requestForm, HttpPostedFileBase[] upload)
        {
            var authUser = User.Identity.GetUserId();
            bool uploadedFileFailure = false;
            string uploadedFileMessage = string.Empty;

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
                request.User = authUser;
                request.StatusId = 1;
                request.ConfirmationCode = Guid.NewGuid().ToString().Substring(0, 9);

                db.Request.Add(request);

                try
                {
                    db.SaveChanges();

                    int requestId = GetRequestId(request.ConfirmationCode.ToString());

                    if (requestId != 0)
                    {
                        AttachmentModel requestAttachment = new AttachmentModel();

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
                                        requestAttachment.RequestId = requestId;
                                        requestAttachment.File = Path.GetFileName(file.FileName);
                                        requestAttachment.ContentType = file.ContentType;

                                        using (var reader = new BinaryReader(file.InputStream))
                                        {
                                            requestAttachment.Content = reader.ReadBytes(file.ContentLength);

                                            db.Attachment.Add(requestAttachment);
                                        }

                                        uploadedFileMessage = upload.Count().ToString() + " file(s) uploaded.";
                                    }
                                    catch (Exception ex)
                                    {
                                        uploadedFileMessage = ex.Message.ToString();
                                        uploadedFileFailure = true;
                                    }
                                }                                
                            }

                            db.SaveChanges();
                        }                        

                        //string callbackUrl = SendEmailConfirmationTokenAsync(authUser, requestId, "Confirm your request.");

                        return RedirectToAction(nameof(CreateConfirmation), new { message = uploadedFileMessage, uploadedFile = uploadedFileFailure });
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            // Sent to error page
                            // TempData["notice"] = "Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage;
                        }
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public ActionResult CreateConfirmation(string fileMessage, bool uploadedFile = false)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("Your request has been created successfully.  It will be reviewed by your administrator who will notify you of the outcome.");

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