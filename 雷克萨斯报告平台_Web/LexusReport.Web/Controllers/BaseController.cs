using Aliyun.OpenServices.OpenStorageService;
using Infragistics.Documents.Excel;
using SevenZip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using LexusReport.Web.Attributes;
using LexusReport.Web.ClientService;
using Lexus.Report.Web.Models;
using ICSharpCode.SharpZipLib.Zip;
using System.Threading;
using System.Configuration;
using LexusReport.Web.App_Start;
using Lexus.Web.Common;

namespace LexusReport.Web.Controllers
{
    [CustomHandleError]
    [AuthenAdmin]
    public class BaseController : Controller
    {
        protected ServiceSoapClient _client;
       
        protected int _countPerPage = 15;
        protected const string accessid = "3JkljJxvXgjLz80X";
        protected const string accessKey = "L2ERHORPk3WkjqfGUb27RlxvT8x5f3";
        protected const string endpoin = "http://oss-cn-beijing.aliyuncs.com";
        public BaseController()
        {
            _client = new ServiceSoapClient("ServiceSoap");
        }
        protected void SetServiceUrl()
        {
            // 登陆者权限是密采审核或者是经销商登陆选择了神秘客的时候，链接地址设置为LEXUSReportMysteryServer
            if ((UserInfo != null && UserInfo.RoleTypeCode == "Max_Mystery")|| ReportType == "Mystery")
            {
                _client.Endpoint.Address = new System.ServiceModel.EndpointAddress("http://60.205.5.60:8000/LEXUSReportMysteryServer/service.asmx");
            }
        }

        protected UserInfoDto UserInfo
        {
            get
            {
                return (UserInfoDto)Session["LoginUser"];
            }
        }
        protected string ReportType
        {
            get
            {
                if (Session["ReportType"] == null) { return ""; }
                else{
                return Session["ReportType"].ToString();}
            }
        }
        protected void CalcPages(int total)
        {
            int pages = total % _countPerPage == 0 ? total / _countPerPage : (total / _countPerPage + 1);
            ViewBag.Total = total;
            ViewBag.Pages = pages;
        }

        /// <summary>
        /// 当前月所在的季度
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public string ConvertMonthToQuarter()
        {
            try
            {
                return ConfigurationSettings.AppSettings["ProjectCode"];
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        protected void ExportExcel<T>(string excelName, List<ColumModel> excelColumList, List<T> dataList)
        {
            Workbook book = new Workbook(WorkbookFormat.Excel97To2003);
            Worksheet sheet = book.Worksheets.Add(excelName);

            #region 创建列头

            for (int k = 0; k < excelColumList.Count; k++)
            {
                ColumModel colModel = excelColumList[k];
                sheet.Rows[0].Cells[k].Value = colModel.name;
                sheet.Rows[1].Cells[k].Value = colModel.label;

                if (colModel.hidden == true)
                {
                    sheet.Columns[k].Hidden = true;
                }

                string align = colModel.align;
                if (!string.IsNullOrEmpty(align))
                {
                    if (align.ToLower() == "left")
                    {
                        sheet.Columns[k].CellFormat.Alignment = HorizontalCellAlignment.Left;
                    }
                    else if (align.ToLower() == "right")
                    {
                        sheet.Columns[k].CellFormat.Alignment = HorizontalCellAlignment.Right;
                    }
                    else
                    {
                        sheet.Columns[k].CellFormat.Alignment = HorizontalCellAlignment.Center;
                    }
                }

                sheet.Columns[k].Width = colModel.width * 35;
            }

            sheet.Rows[0].Hidden = true;

            #endregion

            #region 创建数据

            if (dataList != null && dataList.Count > 0)
            {
                T genericObject = default(T);
                for (int r = 0; r < dataList.Count; r++)
                {
                    for (int c = 0; c < excelColumList.Count; c++)
                    {
                        ColumModel colModel = excelColumList[c];

                        genericObject = dataList[r];
                        Type type = genericObject.GetType();
                        PropertyInfo propertyInfo = type.GetProperty(colModel.name); //获取指定名称的属性
                        if (propertyInfo != null)
                        {
                            object value = propertyInfo.GetValue(genericObject, null); //获取属性值
                            if (value is bool)
                            {
                                sheet.Rows[r + 2].Cells[c].Value = (bool)value ? "√" : "×";
                            }
                            else
                            {
                                sheet.Rows[r + 2].Cells[c].Value = value;
                            }
                        }
                    }
                }
            }

            #endregion

            string fileName = excelName + @".xls";
            //保存excel文件
            string dirPath = this.Server.MapPath("~/Temp/");
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if (!dir.Exists)
            {
                dir.Create();
            }
            book.Save(dirPath + fileName);

            DownloadExcel(fileName, dirPath + fileName, true);
        }
        protected void DownloadExcel(string excelName, string filePath, bool isDeleteAfterDownload = false)
        {
            FileStream stream = new FileStream(filePath, FileMode.Open);
            if (stream == null) return;
            if (string.IsNullOrEmpty(excelName))
            {
                excelName = "GridtoExcel" + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            byte[] bytes = new byte[(int)stream.Length];
            stream.Position = 0;
            stream.Read(bytes, 0, bytes.Length);
            stream.Close();
            Response.Clear();
            Response.Charset = "UTF-8";
            Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
            Response.AddHeader("content-type", "application/x-msdownload");
            Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(excelName, Encoding.GetEncoding("UTF-8")));
            Response.BinaryWrite(bytes);
            Response.End();
            if (isDeleteAfterDownload)
            {
                System.IO.File.Delete(filePath);
            }
        }

        public ActionResult DownloadFile(string file, string fileName)
        {
            string path = HttpContext.Server.MapPath(file);
            FileInfo fileInfo = new FileInfo(file);
            string contentType = "application/x-msdownload";
            fileName = fileName == null ? fileInfo.Name : fileName;

            string agent = Request.UserAgent;
            Encoding encoding = System.Text.Encoding.Default;
            if (agent != null && agent.ToLower().Contains("firefox"))
            {
                encoding = System.Text.Encoding.GetEncoding("GB2312");
            }
            else
            {
                fileName = Server.UrlEncode(fileInfo.Name);
            }
            Response.ContentEncoding = encoding;

            return this.File(path, contentType, fileName);
        }

        protected bool SevenZipCompress(List<ShopDto> fileNames, string foler, string folderToZip, string zipedFile, int level)
        {
            if (!Directory.Exists(folderToZip))
            {
                return false;
            }

            try
            {
                string newZipFolder = zipedFile.Replace(Path.GetExtension(zipedFile), @"\");
                if (!Directory.Exists(newZipFolder))
                {
                    Directory.CreateDirectory(newZipFolder);
                }
                //打包文件拷贝到一个新文件夹
                foreach (ShopDto shop in fileNames)
                {
                    string file = Path.Combine(folderToZip, foler, shop.FileName);
                    string extension = string.Empty;
                    if (!System.IO.File.Exists(file))
                    {
                        continue;
                    }
                    else
                    {
                        System.IO.File.Copy(file, newZipFolder + shop.FileName, true);
                    }
                }

                SevenZipCompressor.SetLibraryPath(Server.MapPath(@"~/bin/7z64.dll"));
                SevenZipCompressor sevenZipCom = new SevenZipCompressor();
                sevenZipCom.CompressDirectory(newZipFolder, zipedFile);

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error("SevenZipCompress Error !", ex);
                return false;
            }
        }
        public bool ZipCompress(List<ShopDto> fileNames, string foler, string folderToZip, string zipedFile)
        {
            if (!Directory.Exists(folderToZip))
            {
                return false;
            }
            try
            {
                string newZipFolder = zipedFile.Replace(Path.GetExtension(zipedFile), @"\");
                if (!Directory.Exists(newZipFolder))
                {
                    Directory.CreateDirectory(newZipFolder);
                }
                //打包文件拷贝到一个新文件夹
                foreach (ShopDto shop in fileNames)
                {
                    string file = Path.Combine(folderToZip, foler, shop.FileName);
                    string extension = string.Empty;
                    if (!System.IO.File.Exists(file))
                    {
                        continue;
                    }
                    else
                    {
                        System.IO.File.Copy(file, newZipFolder + shop.FileName, true);
                    }
                }
                CompressFile(newZipFolder, zipedFile);

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error("ZipCompress error", ex);
                return false;
            }
        }
        /// <summary>
        /// 把指定的文件夹下面的文件压缩打包
        /// </summary>
        /// <param name="zipPath"></param>
        /// <param name="zipFile"></param>
        /// <returns></returns>
        public bool CompressFile(string zipPath, string zipFile)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(zipPath);
                FileInfo[] fileInfo = directoryInfo.GetFiles();
                DateTime dt = DateTime.Now;
                List<FileInfo> logsInOneDay = new List<FileInfo>();
                for (int i = 0; i < fileInfo.Length; i++)
                {
                    if (fileInfo[i].Name.Substring(fileInfo[i].Name.Length - 3) != "zip")
                    {
                        logsInOneDay.Add(fileInfo[i]);
                    }
                }
                if (logsInOneDay.Count > 0)
                {
                    try
                    {
                        Compress(logsInOneDay, zipFile, 9, 100);
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="fileNames"></param>
        /// <param name="GzipFileName"></param>
        /// <param name="CompressionLevel"></param>
        /// <param name="SleepTimer"></param>
        public static void Compress(List<FileInfo> fileNames, string GzipFileName, int CompressionLevel, int SleepTimer)
        {
            ZipOutputStream s = new ZipOutputStream(System.IO.File.Create(GzipFileName));
            try
            {
                s.SetLevel(CompressionLevel);   //0 - store only to 9 - means best compression
                foreach (FileInfo file in fileNames)
                {
                    FileStream fs = null;
                    try
                    {
                        fs = file.Open(FileMode.Open, FileAccess.ReadWrite);
                    }
                    catch
                    {
                        continue;
                    }
                    // 将文件分批读入缓冲区
                    byte[] data = new byte[2048];
                    int size = 2048;
                    ZipEntry entry = new ZipEntry(Path.GetFileName(file.Name));
                    entry.DateTime = (file.CreationTime > file.LastWriteTime ? file.LastWriteTime : file.CreationTime);
                    s.PutNextEntry(entry);
                    while (true)
                    {
                        size = fs.Read(data, 0, size);
                        if (size <= 0) break;
                        s.Write(data, 0, size);
                    }
                    fs.Close();
                    Thread.Sleep(SleepTimer);
                }
            }
            finally
            {
                s.Finish();
                s.Close();
            }
        }

        public ActionResult ProjectSearch()
        {
            ProjectDto[] projects = _client.ProjectSearch();
            return Json(projects, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EmailSend(string subject, string content, string mailToStr)
        {
            string[] mailCC = null;
            string[] mailTo = mailToStr.Split(',');
            List<string> attachPaths = new List<string>();
            try
            {
                //准备发送邮件对象
                ISendMail sendMail = new UseNetMail();
                sendMail.CreateHost(new ConfigHost()
                {
                    EnableSsl = false,
                    Username = ConfigurationSettings.AppSettings["EmailUserName"],
                    Password = ConfigurationSettings.AppSettings["EmailPassword"],
                });
                sendMail.CreateMultiMail(new ConfigMail()
                {
                    From = ConfigurationSettings.AppSettings["EmailUserName"],
                    To = mailTo,
                    CC = mailCC,
                    Subject = subject,
                    Body = content,
                    Attachments = attachPaths.ToArray()
                });

                sendMail.SendMail();
            }
            catch (Exception)
            {


            }
            return Json("");
        }
        public string smallAreaCodeGet()
        {
            if (UserInfo.RoleTypeCode == "02")
            {
                return UserInfo.SmallAreaList[0].AreaCode;
            }
            else
            {
                return "";
            }
        }
    }
}