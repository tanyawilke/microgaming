using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microgaming.Helpers;

namespace Microgaming.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase[] files)
        {
            TempData["notice"] = "File(s) saved.";
            bool fileIsTooLarge = false;
            bool fileExtensionInvalid = false;

            //Ensure model state is valid  
            if (ModelState.IsValid)
            {   //iterating through multiple file collection   
                foreach (HttpPostedFileBase file in files)
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

                        if (maximumAttachmentSize.AllowedFileSize(files.Count(), file.ContentLength))
                        {
                            fileIsTooLarge = true;
                            TempData["notice"] = "A single attachment cannot exceed than 3MB and the total attachment size cannot exceed 15MB.";
                        }

                        if ((!fileExtensionInvalid) && (!fileIsTooLarge))
                        {
                            try
                            {
                                var InputFileName = Path.GetFileName(file.FileName);
                                var ServerSavePath = Path.Combine(Server.MapPath("/UploadedFiles") + InputFileName);

                                //Save file to server folder  
                                file.SaveAs(ServerSavePath);
                                //assigning file uploaded status to ViewBag for showing message to user.  
                                // ViewBag.UploadStatus = files.Count().ToString() + " file(s) uploaded.";
                                TempData["notice"] = files.Count().ToString() + " file(s) uploaded.";
                            }
                            catch (FileLoadException fEx)
                            {
                                TempData["notice"] = "A single attachment cannot exceed than 3MB and the total attachment size cannot exceed 15MB.";
                            }
                            catch (Exception ex)
                            {
                                TempData["notice"] = ex.Message.ToString();
                            }
                        }
                    }
                }
            }

            return View();
        }
    }
}