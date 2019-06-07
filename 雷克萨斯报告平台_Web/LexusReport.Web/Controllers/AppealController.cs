using Aliyun.OpenServices.OpenStorageService;
using Infragistics.Documents.Excel;
using LexusReport.Web.App_Start;
using LexusReport.Web.ClientService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LexusReport.Web.Controllers
{
    public class AppealController : BaseController
    {
        // GET: Appeal
        public ActionResult Index()
        {
            ViewBag.CountPerPage = _countPerPage;
            ViewBag.RoleType = UserInfo.RoleTypeCode;
            ViewBag.CurrentQuarter = ConvertMonthToQuarter();
            ViewBag.ShopCodeForCurrentUser = UserInfo.ShopList[0].ShopCode;
            ViewBag.ShopNameForCurrentUser = UserInfo.ShopList[0].ShopName;

            if (UserInfo.SmallAreaList.Count() > 0)
            {
                ViewBag.AreaCodeForCurrentUser = UserInfo.SmallAreaList[0].AreaCode;
                ViewBag.AreaNameForCurrentUser = UserInfo.SmallAreaList[0].AreaName;
            }
            return View();
        }

        public ActionResult Search(string projectCode = "", string areaCode = "", string groupCode = "", string shopCode = "", string shopCodeKey = "", int pageNum = 1)
        {
            SetServiceUrl();
            var lst = _client.AppealSearch(projectCode, areaCode, smallAreaCodeGet(), groupCode, shopCode, shopCodeKey, pageNum, _countPerPage);
            int total = _client.AppealSearchCount(projectCode, areaCode, smallAreaCodeGet(), groupCode, shopCode, shopCodeKey);
            CalcPages(total);

            return Json(new { AppealDtoList = lst, totalCount = total }, JsonRequestBehavior.AllowGet);
        }
        public void DownloadReport(string projectCode = "", string areaCode = "", string groupCode = "", string shopCode = "", string shopCodeKey = "")
        {
            try
            {
                SetServiceUrl();
                areaCode = areaCode == "undefined" ? "" : areaCode;

                Workbook book = Workbook.Load(Server.MapPath("~") + @"Content\Excel\" + "申诉列表.xlsx", false);

                //填充数据
                Worksheet sheet = book.Worksheets[0];
                List<AppealDto> list = _client.AppealSearch(projectCode, areaCode, smallAreaCodeGet(), groupCode, shopCode, shopCodeKey, 1, 1000000).ToList();
                this.DownloadReport_FillReport(sheet, list);

                //保存excel文件
                string fileName = "申诉列表.xlsx";
                string dirPath = Server.MapPath("~") + @"\Temp\";
                DirectoryInfo dir = new DirectoryInfo(dirPath);
                if (!dir.Exists)
                {
                    dir.Create();
                }
                string filePath = dirPath + fileName;
                book.Save(filePath);

                //下载Excel，并删除临时文件
                this.DownloadExcel(fileName, filePath, true);
            }
            catch (Exception ex)
            {
                LogHelper.Error("下载申诉记录出错", ex);
            }

        }
        private void DownloadReport_FillReport(Worksheet sheet, List<AppealDto> staffInfoList)
        {
            int rowIndex = 1;
            foreach (AppealDto item in staffInfoList)
            {
                //序号
                sheet.GetCell("A" + (rowIndex + 1)).Value = rowIndex.ToString();

                //期号
                sheet.GetCell("B" + (rowIndex + 1)).Value = item.ProjectCode;

                //经销店代码
                sheet.GetCell("C" + (rowIndex + 1)).Value = item.ShopCode;

                //经销店名称
                sheet.GetCell("D" + (rowIndex + 1)).Value = item.ShopName;


                //体系号
                sheet.GetCell("E" + (rowIndex + 1)).Value = item.SubjectCode;

                //指标
                sheet.GetCell("F" + (rowIndex + 1)).Value = item.CheckPoint;

                //申诉理由
                sheet.GetCell("G" + (rowIndex + 1)).Value = item.AppealReason;


                //项目组反馈
                sheet.GetCell("H" + (rowIndex + 1)).Value = item.MaxFeedBackStr;


                //项目组反馈意见
                sheet.GetCell("I" + (rowIndex + 1)).Value = item.MaxFeedBackReason;

                //经销店接受与否
                sheet.GetCell("J" + (rowIndex + 1)).Value = item.ShopAcceptStr;


                //经销店意见
                sheet.GetCell("K" + (rowIndex + 1)).Value = item.ShopAcceptReason;

                //最终意见
                sheet.GetCell("L" + (rowIndex + 1)).Value = item.LEXUSFeedBackStr;

                //申请时间
                sheet.GetCell("M" + (rowIndex + 1)).Value = item.AppealDateTime.HasValue ? item.AppealDateTime.Value.ToString("yyyy-MM-dd") : "";


                rowIndex++;
            }
        }
        public ActionResult AppealCountByArea()
        {
            ViewBag.RoleType = UserInfo.RoleTypeCode;
            ViewBag.CurrentQuarter = ConvertMonthToQuarter();
            ViewBag.ShopCodeForCurrentUser = UserInfo.ShopList[0].ShopCode;
            ViewBag.ShopNameForCurrentUser = UserInfo.ShopList[0].ShopName;
            if (UserInfo.SmallAreaList.Count() > 0)
            {
                ViewBag.AreaCodeForCurrentUser = UserInfo.SmallAreaList[0].AreaCode;
                ViewBag.AreaNameForCurrentUser = UserInfo.SmallAreaList[0].AreaName;
            }

            return View();
        }
        public ActionResult AppealCountByAreaSearch(string projectCode = "")
        {
            var lst = _client.AppealCountByArea(projectCode, "");
            // int total = 100;
            // CalcPages(total);
            return Json(new { AppealCountByArea = lst }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SubjectSearch(string projectCode, string subjectCode)
        {
            SubjectDto dto = _client.SubjectSearch(projectCode, subjectCode).FirstOrDefault();
            return Json(dto == null ? (object)false : dto, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            string shopCode = UserInfo.ShopList[0].ShopCode;
            AppealDto dto = new AppealDto();
            ViewBag.ProjectDtos = _client.ProjectSearch();
            ViewBag.ShopCode = shopCode;
            ViewBag.CurrentQuarter = ConvertMonthToQuarter();
            return PartialView(dto);
        }
        public ActionResult Edit(string projectCode, string shopCode, string subjectCode)
        {
            AppealDto dto = _client.AppealDtlSearch(projectCode, shopCode, subjectCode);
            ViewBag.ShopCode = dto.ShopCode;
            ViewBag.ProjectDtos = _client.ProjectSearch();
            ViewBag.RoleType = UserInfo.RoleTypeCode;
            ViewBag.UserId = UserInfo.UserId;

            return PartialView("Edit", dto);
        }


        public ActionResult CheckAppealSave(AppealDto dto, bool add)
        {
            if (UserInfo.ShopList == null || UserInfo.ShopList.Length == 0)
            {
                throw new Exception("用户不属于经销店，不能申诉登记");
            }
            AppealDto findOne = _client.AppealDtlSearch(dto.ProjectCode, dto.ShopCode, dto.SubjectCode);
            if (findOne != null && !string.IsNullOrWhiteSpace(findOne.AppealReason))
            {
                throw new Exception("此题号已经申诉过，不能再次申诉");
            }
            return Json("");
        }
        public ActionResult AppealSave(AppealDto dto, bool add)
        {
            try
            {
                if (add)
                {
                    string shopCode = UserInfo.ShopList[0].ShopCode;
                    _client.AppealSave(dto.ProjectCode, shopCode, dto.SubjectCode, dto.AppealReason, UserInfo.UserId);
                }
                else
                {
                    string shopCode = dto.ShopCode;
                    bool areaNeedChk = dto.AreaNeedChk.HasValue ? dto.AreaNeedChk.Value : false;
                    if (UserInfo.RoleTypeCode == "Max")
                    {
                        if (dto.LEXUSFeedBack.HasValue)
                        {
                            _client.AppealFeedBackSave(dto.ProjectCode, shopCode, dto.SubjectCode, "LEXUS", dto.LEXUSFeedBack, dto.LEXUSFeedBackReason, UserInfo.UserId, areaNeedChk);
                        }
                        else
                        {
                            _client.AppealFeedBackSave(dto.ProjectCode, shopCode, dto.SubjectCode, "Max", dto.MaxFeedBack, dto.MaxFeedBackReason, UserInfo.UserId, areaNeedChk);
                        }
                    }
                    else if (UserInfo.RoleTypeCode == "Shop")
                    {
                        if (dto.ShopAcceptChk.HasValue)
                        {
                            _client.AppealFeedBackSave(dto.ProjectCode, shopCode, dto.SubjectCode, "Shop", dto.ShopAcceptChk, dto.ShopAcceptReason, UserInfo.UserId, areaNeedChk);
                        }
                    }
                    //else if (UserInfo.RoleTypeCode == "LEXUS")
                    //{
                    //    AppealDto oldOne = _client.AppealDtlSearch(dto.ProjectCode, shopCode, dto.SubjectCode);
                    //    if ((!oldOne.AreaNeedChk.HasValue || !oldOne.AreaNeedChk.Value) && areaNeedChk)
                    //    {//第一次保存 需要区域反馈  发送提醒邮件给区域账户
                    //        string subject = "申诉需要区域反馈-提醒邮件";
                    //        string content = string.Format("期号：{0}  <br> 经销店：{1}  <br> 体系号：{2}  <br> 的申诉记录需要区域反馈，请尽快处理。 ", dto.ProjectCode, shopCode, dto.SubjectCode);
                    //        string emailAddr = "";
                    //        ShopDto[] shopList = _client.AreaInfoByShopCode(shopCode);
                    //        if (shopList.Length > 0)
                    //        {
                    //            emailAddr = string.Join(",", shopList.Select(x => x.AreaEmail));
                    //        }
                    //        if (!string.IsNullOrEmpty(emailAddr))
                    //        {
                    //            EmailSend(subject, content, emailAddr);
                    //        }
                    //    }
                    //    _client.AppealFeedBackSave(dto.ProjectCode, shopCode, dto.SubjectCode, "LEXUS", dto.LEXUSFeedBack, dto.LEXUSFeedBackReason, UserInfo.UserId, areaNeedChk);
                    //}
                    //else if (UserInfo.RoleTypeCode == "02")
                    //{
                    //    _client.AppealFeedBackSave(dto.ProjectCode, shopCode, dto.SubjectCode, "02", dto.AreaFeedBack, dto.AreaFeedBackReason, UserInfo.UserId, areaNeedChk);
                    //}
                    //else if (UserInfo.RoleTypeCode == "01")
                    //{
                    //    _client.AppealFeedBackSave(dto.ProjectCode, shopCode, dto.SubjectCode, "01", dto.BigAreaFeedBack, dto.BigAreaFeedBackReason, UserInfo.UserId, areaNeedChk);
                    //}
                    else if (UserInfo.RoleTypeCode == "S")
                    {
                        if (dto.ShopAcceptChk.HasValue)
                        {
                            _client.AppealFeedBackSave(dto.ProjectCode, shopCode, dto.SubjectCode, "Shop", dto.ShopAcceptChk, dto.ShopAcceptReason, UserInfo.UserId, areaNeedChk);
                        }
                        if (dto.LEXUSFeedBack.HasValue)
                        {
                            _client.AppealFeedBackSave(dto.ProjectCode, shopCode, dto.SubjectCode, "LEXUS", dto.LEXUSFeedBack, dto.LEXUSFeedBackReason, UserInfo.UserId, areaNeedChk);
                        }
                        if (dto.MaxFeedBack.HasValue)
                        {
                            _client.AppealFeedBackSave(dto.ProjectCode, shopCode, dto.SubjectCode, "Max", dto.MaxFeedBack, dto.MaxFeedBackReason, UserInfo.UserId, areaNeedChk);
                        }

                        //_client.AppealFeedBackSave(dto.ProjectCode, shopCode, dto.SubjectCode, "Max", dto.MaxFeedBack, dto.MaxFeedBackReason, UserInfo.UserId, areaNeedChk);
                        //_client.AppealFeedBackSave(dto.ProjectCode, shopCode, dto.SubjectCode, "LEXUS", dto.LEXUSFeedBack, dto.LEXUSFeedBackReason, UserInfo.UserId, areaNeedChk);
                        //_client.AppealFeedBackSave(dto.ProjectCode, shopCode, dto.SubjectCode, "01", dto.AreaFeedBack, dto.AreaFeedBackReason, UserInfo.UserId, areaNeedChk);
                        //_client.AppealFeedBackSave(dto.ProjectCode, shopCode, dto.SubjectCode, "02", dto.AreaFeedBack, dto.AreaFeedBackReason, UserInfo.UserId, areaNeedChk);
                    }
                }

                return Json("", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Error("申诉保存出错", ex);
                return Json("申诉保存出错", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AppealFileSearch(string projectCode, string shopCode, string subjectCode)
        {
            var lst = _client.AppealFileSearch(projectCode, shopCode, subjectCode);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AppealFileSave(string projectCode, string shopCode, string subjectCode, string fileName, string serverName, string fileType)
        {
            _client.AppealFileSave(0, projectCode, shopCode, subjectCode, fileType, fileName, serverName, UserInfo.UserId);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadOSSFile(string filename, string downloadName)
        {
            try
            {

                OssClient ossClient = new OssClient(endpoin, accessid, accessKey);
                OssObject file = ossClient.GetObject("vgic", filename);
                downloadName = string.IsNullOrEmpty(downloadName) ? Path.GetFileName(filename) : downloadName;
                return File(file.Content, "application/octet-stream", downloadName);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public ActionResult DeleteFile(int id, string filename)
        {
            _client.AppealFileDelete(id);
            OssClient ossClient = new OssClient(endpoin, accessid, accessKey);
            ossClient.DeleteObject("vgic", filename);

            return Json(true);
        }
    }
}