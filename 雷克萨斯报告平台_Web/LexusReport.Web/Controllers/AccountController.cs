
using LexusReport.Web.ClientService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace LexusReport.Web.Controllers
{
    public class AccountController : BaseController
    {
        // GET: Account
        public ActionResult Login(string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult Login(string sUserName, string sPassword, string ReturnUrl)
        {
            string loginStatus = _client.DSATReport_Login(sUserName, sPassword);

            if (loginStatus == "1")
            {
                return Json(new { Status = false, sErrMsg = "用户名不存在`！" });
            }
            else if (loginStatus == "2")
            {
                return Json(new { Status = false, sErrMsg = "密码不正确！" });
            }
            //登陆成功 用户信息放到session中
            UserInfoDto userInfo = _client.DSATReport_UserInfoSearch(sUserName);
            ViewBag.Password = userInfo.Password;
            if (userInfo != null)
            {
                FormsAuthentication.SetAuthCookie(userInfo.UserId, false);
                Session["LoginUser"] = userInfo;
                Session["UserId"] = userInfo.UserId;
                if (userInfo.RoleTypeCode == "Shop")
                {
                    ReturnUrl = "/Home/ReportTypeSelect";
                }
                return Json(new { Status = true, sRedirectURL = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) + ReturnUrl });
            }
            else
            {
                return Json(new { Status = false, sErrMsg = "用户信息不存在！" });
            }

        }
        private string StrToMD5(string str)
        {
            byte[] data = Encoding.GetEncoding("GB2312").GetBytes(str);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] OutBytes = md5.ComputeHash(data);

            string OutString = "";
            for (int i = 0; i < OutBytes.Length; i++)
            {
                OutString += OutBytes[i].ToString("x2");
            }
            return OutString.ToLower();
        }

        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            Session["LoginUser"] = null;

            return this.Redirect("~/");
        }
        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(string sOldPassword, string sNewPassword)
        {
            try
            {
                string result = _client.DSATReport_PasswordModify(UserInfo.UserId, sOldPassword, sNewPassword);
                FormsAuthentication.SignOut();
                Session["LoginUser"] = null;

                return Json(new { nAction = 2, sRedirectURL = "/Account/Login" });
            }
            catch (Exception ex)
            {
                return Json(new { nAction = 1, sRedirectURL = "/Home/Index" });
            }
        }

        [HttpPost]
        public ActionResult CheckUserId(string userId)
        {
            string loginStatus = _client.DSATReport_Login(userId, "");
            if (loginStatus == "1")
            {
                return Json(new { Status = false, sErrMsg = "用户名不存在！" });
            }
            return Json(new { Status = true });
        }
        [HttpPost]
        public ActionResult ForgetPassword(string userId)
        {
            string subject = "密码找回申请";
            string content = string.Format("登录名：{0} 的用户忘记了密码，帮忙找回密码，谢谢！ ", userId);
            //string emailAddr = "71443365@qq.com";
            EmailSend(subject, content, ConfigurationSettings.AppSettings["ForgetPasswordEmail"]);

            return Json(new { Status = true });
        }
    }
}