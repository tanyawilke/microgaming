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
        private bool isValidExtension;

        public bool FileExtentionAllowed(string[] fileExtentions)
        {
            if (fileExtentions.Length > 1)
            {
                foreach (string extention in fileExtentions)
                {
                    isValidExtension = ContainsExtension(allowedFileExtensions, extention);
                }
            }
            else
            {
                isValidExtension = ContainsExtension(allowedFileExtensions, fileExtentions[0].ToString());
            }            

            return isValidExtension;
        }

        private bool ContainsExtension(string[] array, string valueToTest)
        {
            bool isContained = false;

            var match = array.FirstOrDefault(stringToCheck => stringToCheck.Contains(valueToTest));

           if (match != null)
            {
                isContained = true;
            }

            return isContained;
        }
    }
}