using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LexusReport.Web.App_Start
{
    public class LogHelper
    {
        private static readonly log4net.ILog loginfo = log4net.LogManager.GetLogger("InfoLog");   //选择<logger name="InfoLog">的配置 

        private static readonly log4net.ILog logerror = log4net.LogManager.GetLogger("ErrorLog");   //选择<logger name="ErrorLog">的配置 


        public static void Info(string info)
        {
            loginfo.Info(info);
        }

        public static void Error(string errMsg, Exception se)
        {
            logerror.Error(errMsg, se);
        }   
    }
}