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
using System.Collections.Generic;

/// <summary>
/// Summary description for UserInfoDto
/// </summary>
public class ProjectDto
{
    public ProjectDto()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public string ProjectCode { get; set; }
    public string ProjectName { get; set; }
    public string Year { get; set; }
    public string Quarter { get; set; }
    public string OrderNO { get; set; }
}
