using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Management;

namespace Microgaming.Helpers
{
    public class GlobalHelper
    {
        const int TimedOutExceptionCode = -2147467259;
        public static bool IsMaxRequestExceededException(Exception e)
        {
            // unhandled errors = caught at global.ascx level
            // http exception = caught at page level

            //

            //if (httpException.WebEventCode == WebEventCodes.RuntimeErrorPostTooLarge)
            //{
            //    //handle the error
            //    TempData["notice"] = "Too big a file, dude"; //for example
            //}

            Exception ex = null;

            var httpException = ex as HttpException ?? ex.InnerException as HttpException;

            if (httpException.WebEventCode == WebEventCodes.RuntimeErrorPostTooLarge)
            {
                //handle the error
                //    TempData["notice"] = "Too big a file, dude"; //for example
            }
            else
            {
                ex = e;
            }

            var http = ex as HttpException;

            if (http != null && http.ErrorCode == TimedOutExceptionCode)
            {
                // hack: no real method of identifying if the error is max request exceeded as 
                // it is treated as a timeout exception
                if (http.StackTrace.Contains("GetEntireRawContent"))
                {
                    // MAX REQUEST HAS BEEN EXCEEDED
                    return true;
                }
            }

            return false;
        }
    }
}