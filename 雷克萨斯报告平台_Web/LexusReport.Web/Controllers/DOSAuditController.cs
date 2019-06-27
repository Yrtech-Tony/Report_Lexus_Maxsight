using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using LexusReport.Web.ClientService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LexusReport.Web.Controllers
{
    public class DOSAuditController : BaseController
    {
        // GET: CSSOEM
        public ActionResult Index()
        {
            return View();
        }
        #region 季度报告
        public ActionResult QuarterlyReport()
        {
            ViewBag.CountPerPage = _countPerPage;
            ViewBag.RoleType = UserInfo.RoleTypeCode;
            ViewBag.CurrentQuarter = ConvertMonthToQuarter();
            ViewBag.ShopCodeForCurrentUser = UserInfo.ShopList[0].ShopCode;
            ViewBag.ShopNameForCurrentUser = UserInfo.ShopList[0].ShopName;
            if (UserInfo.GroupList.Count() > 0)
            {
                ViewBag.GroupCodeForCurrentUser = UserInfo.GroupList[0].GroupCode;
                ViewBag.GroupNameForCurrentUser = UserInfo.GroupList[0].GroupName;
            }
            if (UserInfo.SmallAreaList.Count() > 0)
            {
                ViewBag.AreaCodeForCurrentUser = UserInfo.SmallAreaList[0].AreaCode;
                ViewBag.AreaNameForCurrentUser = UserInfo.SmallAreaList[0].AreaName;
            }
            if (UserInfo.BigAreaList.Count() > 0)
            {
                ViewBag.AreaCodeForCurrentUser = UserInfo.BigAreaList[0].AreaCode;
                ViewBag.AreaNameForCurrentUser = UserInfo.BigAreaList[0].AreaName;
            }
            return View();
        }

        /// <summary>
        /// 季度报告查询分页
        /// </summary>
        /// <param name="areaCode"></param>
        /// <param name="groupCode"></param>
        /// <param name="shopCode"></param>
        /// <param name="year"></param>
        /// <param name="quarter"></param>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        public ActionResult QuarterSearch(string areaCode, string groupCode, string shopCode, string projectCode, string shopCodeKey, int pageNum)
        {
            //  projectCode = "2017Q1";
            string reportPath = "";
            string downLoadPath = "";
            if (ReportType == "Mystery" || UserInfo.RoleTypeCode == "Max_Mystery")
            {
                reportPath = HttpContext.Server.MapPath("~/ReportFiles/DOSAudit/QuarterReport_Mystery");//季度报告所在的文件路径 
                downLoadPath = "DownloadFile?file=~/ReportFiles/DOSAudit/QuarterReport_Mystery/";
            }
            else
            {
                reportPath = HttpContext.Server.MapPath("~/ReportFiles/DOSAudit/QuarterReport");//季度报告所在的文件路径
                downLoadPath = "DownloadFile?file=~/ReportFiles/DOSAudit/QuarterReport/";
            }
            List<ShopDto> resultListTemp = GetFileList(areaCode, groupCode, shopCode, projectCode, shopCodeKey, reportPath);//获取符合条件的所有季度报告
            List<ShopDto> resultList = ((from u in resultListTemp orderby u.UploadDate select u).Skip(_countPerPage * (pageNum - 1)).Take(_countPerPage)).ToList<ShopDto>();//分页
            return Json(new { shopInfoList = resultList, totalCount = resultListTemp.Count, downLoadPath = downLoadPath }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 指导手册
        public ActionResult DealerGuide()
        {
            ViewBag.CountPerPage = _countPerPage;
            ViewBag.RoleType = UserInfo.RoleTypeCode;
            ViewBag.CurrentQuarter = ConvertMonthToQuarter();
            return View();
        }
        public ActionResult DealerGuideSearch(int pageNum)
        {
            string reportPath = "";
            string downLoadPath = "";
            if (ReportType == "Mystery" || UserInfo.RoleTypeCode == "Max_Mystery")
            {
                reportPath = HttpContext.Server.MapPath("~/ReportFiles/DOSAudit/DealerGuide_Mystery");//指导手册所在的文件路径
                downLoadPath = "DownloadFile?file=~/ReportFiles/DOSAudit/DealerGuide_Mystery/";
            }
            else
            {
                reportPath = HttpContext.Server.MapPath("~/ReportFiles/DOSAudit/DealerGuide");//指导手册所在的文件路径
                downLoadPath = "DownloadFile?file=~/ReportFiles/DOSAudit/DealerGuide/";
            }

            List<ShopDto> resultListTemp = new List<ShopDto>();
            DirectoryInfo dataDir = new DirectoryInfo(reportPath);
            FileInfo[] filesInfos = dataDir.GetFiles();
            foreach (FileInfo fileInfo in filesInfos)
            {
                ShopDto shopDto = new ShopDto();
                resultListTemp.Add(new ShopDto()
                {
                    UploadDate = fileInfo.CreationTime,
                    FileName = fileInfo.Name,
                });
            }
            List<ShopDto> resultList = ((from u in resultListTemp orderby u.UploadDate select u).Skip(_countPerPage * (pageNum - 1)).Take(_countPerPage)).ToList<ShopDto>();//分页
            return Json(new { shopInfoList = resultList, totalCount = resultListTemp.Count, downLoadPath = downLoadPath }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 检查标准
        public ActionResult Inspection()
        {
            ViewBag.CountPerPage = _countPerPage;
            ViewBag.RoleType = UserInfo.RoleTypeCode;
            ViewBag.CurrentQuarter = ConvertMonthToQuarter();
            return View();
        }
        public ActionResult InspectionSearch(int pageNum)
        {
            string reportPath = "";
            string downLoadPath = "";
            if (ReportType == "Mystery" || UserInfo.RoleTypeCode == "Max_Mystery")
            {
                reportPath = HttpContext.Server.MapPath("~/ReportFiles/DOSAudit/Inspection_Mystery");//检查标准路径
                downLoadPath = "DownloadFile?file=~/ReportFiles/DOSAudit/Inspection_Mystery/";
            }
            else
            {
                reportPath = HttpContext.Server.MapPath("~/ReportFiles/DOSAudit/Inspection");//检查标准路径
                downLoadPath = "DownloadFile?file=~/ReportFiles/DOSAudit/Inspection/";
            }
            List<ShopDto> resultListTemp = new List<ShopDto>();
            DirectoryInfo dataDir = new DirectoryInfo(reportPath);
            FileInfo[] filesInfos = dataDir.GetFiles();
            foreach (FileInfo fileInfo in filesInfos)
            {
                ShopDto shopDto = new ShopDto();
                resultListTemp.Add(new ShopDto()
                {
                    UploadDate = fileInfo.LastWriteTime,
                    FileName = fileInfo.Name,
                });
            }
            List<ShopDto> resultList = ((from u in resultListTemp orderby u.ShopCode select u).Skip(_countPerPage * (pageNum - 1)).Take(_countPerPage)).ToList<ShopDto>();//分页
            return Json(new { shopInfoList = resultList, totalCount = resultListTemp.Count, downLoadPath = downLoadPath }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 获取文件列表 季度或者月度
        public List<ShopDto> GetFileList(string areaCode, string groupCode, string shopCode, string projectCode, string shopCodeKey, string reportPath)
        {
            SetServiceUrl();
            List<ShopDto> resultListTemp = new List<ShopDto>();
            List<ShopDto> list = _client.DSATReport_SearchReportShopInfo(areaCode, smallAreaCodeGet(), groupCode, shopCode, shopCodeKey, 1, 100000).ToList();
            DirectoryInfo dataDir = new DirectoryInfo(reportPath);
            FileInfo[] filesInfo = dataDir.GetFiles();
            try
            {

                // 添加经销店的文件
                foreach (ShopDto strShop in list)
                {
                    foreach (FileInfo file in filesInfo)
                    {
                        // 季度或月份没有选择的情况下只判断年份和经销店代码即可
                        //if (string.IsNullOrEmpty(quarterormonth) || quarterormonth == "Q")
                        //{
                        //    if (file.Name.Split('_')[1] == year && file.Name.Split('_')[0].Trim() == strShop.ShopCode.Trim())
                        //    {
                        //        ShopDto result = new ShopDto();
                        //        result.ShopCode = strShop.ShopCode;
                        //        result.ShopName = strShop.ShopName;
                        //        result.AreaName = strShop.AreaName;
                        //        result.GroupName = strShop.GroupName;
                        //        result.ShopType = strShop.ShopType;
                        //        result.FileName = file.Name;
                        //        result.UploadDate = file.LastWriteTime;

                        //        resultListTemp.Add(result);
                        //    }
                        //}
                        //else // 季度或月份选择的情况下
                        //{
                        //if (file.Name.Split('_')[0].Substring(0, 6) == projectCode
                        //    && file.Name.Split('_')[1].Trim() == strShop.ShopCode.Trim())
                        //{
                        //    ShopDto result = new ShopDto();
                        //    result.ShopCode = strShop.ShopCode;
                        //    result.ShopName = strShop.ShopName;
                        //    //result.AreaName = strShop.AreaName;
                        //    result.BigAreaCode = strShop.BigAreaCode;
                        //    result.GroupName = strShop.GroupName;
                        //    //result.ShopType = strShop.ShopType;
                        //    result.FileName = file.Name;
                        //    result.UploadDate = file.LastWriteTime;

                        //    resultListTemp.Add(result);
                        //}
                        if (file.Name.Trim().Contains(projectCode.Trim())
                           && file.Name.Trim().Contains(strShop.ShopCode.Trim()))
                        {
                            ShopDto result = new ShopDto();
                            result.ShopCode = strShop.ShopCode;
                            result.ShopName = strShop.ShopName;
                            //result.AreaName = strShop.AreaName;
                            result.BigAreaCode = strShop.BigAreaCode;
                            result.GroupName = strShop.GroupName;
                            //result.ShopType = strShop.ShopType;
                            result.FileName = file.Name;
                            result.UploadDate = file.LastWriteTime;

                            resultListTemp.Add(result);
                        }
                        //}

                    }
                }
                #region 添加区域的文件
                //if (!string.IsNullOrEmpty(areaCode))
                //{
                //    foreach (FileInfo file in filesInfo)
                //    {
                //        if (string.IsNullOrEmpty(quarterormonth) || quarterormonth == "Q")
                //        {
                //            if (file.Name.Split('_')[1] == year && file.Name.Split('_')[0].Trim() == areaCode.Trim())
                //            {
                //                ShopDto result = new ShopDto();
                //                result.ShopCode = "";
                //                result.ShopName = "";
                //                result.AreaName = areaCode;
                //                result.GroupName = "";
                //                result.ShopType = "";
                //                result.FileName = file.Name;
                //                result.UploadDate = file.LastWriteTime;
                //                resultListTemp.Add(result);
                //            }
                //        }
                //        else
                //        {
                //            if (file.Name.Split('_')[1] == year
                //                && (file.Name.Split('_')[2] == quarterormonth || file.Name.Split('_')[2] == "Q" + quarterormonth)
                //                 && file.Name.Split('_')[0].Trim() == areaCode.Trim())
                //            {
                //                ShopDto result = new ShopDto();
                //                result.ShopCode = "";
                //                result.ShopName = "";
                //                result.AreaName = areaCode;
                //                result.GroupName = "";
                //                result.ShopType = "";
                //                result.FileName = file.Name;
                //                result.UploadDate = file.LastWriteTime;
                //                resultListTemp.Add(result);
                //            }
                //        }
                //    }
                //}
                #endregion
                #region 添加全国文件
                //if (string.IsNullOrEmpty(areaCode) && string.IsNullOrEmpty(groupCode) && string.IsNullOrEmpty(shopCode))
                //{
                //    foreach (FileInfo file in filesInfo)
                //    {
                //        if (string.IsNullOrEmpty(quarterormonth) || quarterormonth == "Q")
                //        {
                //            if (file.Name.Split('_')[1] == year && (file.Name.Split('_')[0].Trim() == "Nation"
                //                                                    || file.Name.Split('_')[0].Trim() == "East"
                //                                                    || file.Name.Split('_')[0].Trim() == "Central"
                //                                                    || file.Name.Split('_')[0].Trim() == "North"
                //                                                    || file.Name.Split('_')[0].Trim() == "Phaeton"
                //                                                    || file.Name.Split('_')[0].Trim() == "South"
                //                                                    || file.Name.Split('_')[0].Trim() == "West"))
                //            {
                //                ShopDto result = new ShopDto();
                //                result.ShopCode = "";
                //                result.ShopName = "";
                //                result.AreaName = "";
                //                result.GroupName = "";
                //                result.ShopType = "";
                //                result.FileName = file.Name;
                //                result.UploadDate = file.LastWriteTime;
                //                resultListTemp.Add(result);
                //            }
                //        }
                //        else
                //        {
                //            if (file.Name.Split('_')[1] == year
                //                && (file.Name.Split('_')[2] == quarterormonth || file.Name.Split('_')[2] == "Q" + quarterormonth)
                //                 && (file.Name.Split('_')[0].Trim() == "Nation"
                //                                                    || file.Name.Split('_')[0].Trim() == "East"
                //                                                    || file.Name.Split('_')[0].Trim() == "Central"
                //                                                    || file.Name.Split('_')[0].Trim() == "North"
                //                                                    || file.Name.Split('_')[0].Trim() == "Phaeton"
                //                                                    || file.Name.Split('_')[0].Trim() == "South"
                //                                                    || file.Name.Split('_')[0].Trim() == "West"))
                //            {
                //                ShopDto result = new ShopDto();
                //                result.ShopCode = "";
                //                result.ShopName = "";
                //                result.AreaName = "";
                //                result.GroupName = "";
                //                result.ShopType = "";
                //                result.FileName = file.Name;
                //                result.UploadDate = file.LastWriteTime;
                //                resultListTemp.Add(result);
                //            }
                //        }
                //    }
                //}
                #endregion
            }
            catch (Exception)
            {
            }
            return resultListTemp;
        }

        #endregion
        #region 打包下载文件
        public ActionResult DownloadFiles(string areaCode, string groupCode, string shopCode, string projectCode, string shopCodeKey, string reportType)
        {
            //projectCode = "2017Q1";
            try
            {
                string type = "";
                 
                string reportPath = HttpContext.Server.MapPath("~/ReportFiles/DOSAudit/");
                if (ReportType == "Mystery" || UserInfo.RoleTypeCode == "Max_Mystery")
                {
                    type = "QuarterReport_Mystery";
                }
                else
                {
                    type = "QuarterReport";
                }
                List<ShopDto> resultListTemp = GetFileList(areaCode, groupCode, shopCode, projectCode, shopCodeKey, reportPath + @"\" + type);
                string temp = Path.Combine(reportPath, @"TEMP\" + "LEXUSReport" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".zip");
                if (System.IO.File.Exists(temp))
                {
                    System.IO.File.Delete(temp);
                }
                if (!ZipCompress(resultListTemp, type, reportPath, temp))
                {
                    throw new Exception("打包文件失败！");
                }

                return Json(new { Status = true, File = temp.Replace(HttpContext.Server.MapPath("~"), "/") }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, ErrorMsg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion


    }
}