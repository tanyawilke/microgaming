using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace FinanceRequest.Helpers
{
    public class AllowedFileExtensionsHelper
    {
        private readonly string[] allowedFileExtensions = ConfigurationManager.AppSettings["FileExtensionsAllowed"].Split(',');

        public bool FileExtentionAllowed(string[] fileExtentions)
        {
            foreach (var extention in fileExtentions)
            {
                if (!allowedFileExtensions.Contains(extention))
                {
                    return false;
                }
            }

            return true;
        }
    }
}