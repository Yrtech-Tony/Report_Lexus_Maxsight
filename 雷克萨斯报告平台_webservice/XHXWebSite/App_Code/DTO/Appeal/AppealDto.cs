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
public class AppealDto
{
    public AppealDto()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public string ProjectCode { get; set; }
    public string ShopCode { get; set; }
    public string ShopName { get; set; }
    public string SubjectCode { get; set; }
    public string CheckPoint { get; set; }
    public string AppealReason { get; set; }

    public bool? MaxFeedBack { get; set; }
    public string MaxFeedBackStr { get; set; }
    public string MaxFeedBackReason { get; set; }

    public bool? ShopAcceptChk { get; set; }
    public string ShopAcceptStr { get; set; }
    public string ShopAcceptReason { get; set; }

    public bool? LEXUSFeedBack { get; set; }
    public string LEXUSFeedBackStr { get; set; }
    public string LEXUSFeedBackReason { get; set; }
    public bool? AreaNeedChk { get; set; }


    public bool? AreaFeedBack { get; set; }
    public string AreaFeedBackStr { get; set; }
    public string AreaFeedBackReason { get; set; }
    public DateTime? AppealDateTime { get; set; }

    public bool? BigAreaFeedBack { get; set; }
    public string BigAreaFeedBackStr { get; set; }
    public string BigAreaFeedBackReason { get; set; }
    public DateTime? BigAppealDateTime { get; set; }

    public bool? LastFeedBack { get; set; }
    public string LastFeedBackStr { get; set; }
}
