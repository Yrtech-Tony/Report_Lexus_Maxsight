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
/// Summary description for AppealCountByArea
/// </summary>
public class AppealCountByArea
{
    public AppealCountByArea()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public string ProjectCode { get; set; }
    public string AreaCode { get; set; }
    public string AreaName { get; set; }
    public string SubjectCode { get; set; }
    public string CheckPoint { get; set; }
    public int AppealCount { get; set; }
    public int AppealSuccessCount { get; set; }
    public int AreaCount { get; set; }

}
