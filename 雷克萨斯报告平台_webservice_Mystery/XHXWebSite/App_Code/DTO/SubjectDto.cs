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
/// Summary description for SubjectDto
/// </summary>
public class SubjectDto
{
    public SubjectDto()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public string ProjectCode { get; set; }
    public string SubjectCode { get; set; }
    public string CheckPoint { get; set; }
}
