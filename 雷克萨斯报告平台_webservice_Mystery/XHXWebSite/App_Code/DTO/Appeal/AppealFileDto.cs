using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

/// <summary>
/// Summary description for AppealDto
/// </summary>
public class AppealFileDto
{
    public AppealFileDto()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public int SeqNO { get; set; }
    public string ProjectCode { get; set; }
    public string ShopCode { get; set; }
    public string SubjectCode { get; set; }
    public string FileType { get; set; }
    public string FileTypeName { get; set; }
    public string FileName { get; set; }
    public string ServerFileName { get; set; }
    public string InUserId { get; set; }
    public DateTime InDateTime { get; set; }
   

}
