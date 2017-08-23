using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;

namespace SB.AR.AppWeb.Utility
{
    public class Logging
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        
            
        public static void LogMessage(string message)
        {
            log.Debug(message);
        }
        public static void LogErrorException(Exception ex)
        {
            if (log.IsErrorEnabled)
            {
                log.Error(ex.Message);
            }
        }
        public static void LogErrorException(Exception ex,string message)
        {
            if (log.IsErrorEnabled)
            {
                if(ex == null)
                {
                    log.Error(message);

                }
                else
                {
                    log.Error(message + " : " + ex.ToString());
                }
                
            }
        }
    }
}