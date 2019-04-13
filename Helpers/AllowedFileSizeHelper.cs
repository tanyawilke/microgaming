using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Microgaming.Helpers
{
    public class AllowedFileSizeHelper
    {
        private readonly int individualFileSizeMax = Int32.Parse(ConfigurationManager.AppSettings["DocUploadValidationSize"]);
        private readonly int cumulativeFileSizeMax = Int32.Parse(ConfigurationManager.AppSettings["CumulativeDocUploadValidationSize"]);

        public bool AllowedFileSize(int numberOfFiles, int fileLength)
        {
            if (numberOfFiles > 1)
            {
                return CumulativeFileSize(fileLength);
            }
            else
            {
                return IndividualFileSize(fileLength);
            }
        }

        public bool IndividualFileSize(int fileSize)
        {
            if (fileSize > individualFileSizeMax)
            {
                return true;
            }

            return false;
        }

        public bool CumulativeFileSize(int fileSize)
        {
            if (fileSize > cumulativeFileSizeMax)
            {
                return true;
            }

            return false;
        }
    }
}