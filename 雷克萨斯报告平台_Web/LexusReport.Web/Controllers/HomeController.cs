using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LexusReport.Web.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index(string reportType)
        {
            ViewBag.Password = UserInfo.Password;
            Session["ReportType"] = reportType;
            ViewBag.ReportType = reportType;
            ViewBag.RoleTypeCode = UserInfo.RoleTypeCode;
            return View();
        }
        public ActionResult ReportTypeSelect()
        {
            //ViewBag.ReportType = reportType;
            return View();
        }
        
    }
}