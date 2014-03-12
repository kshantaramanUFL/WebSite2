using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.IO;

public partial class Default2 : System.Web.UI.Page
{
    string path = "";
    protected void UploadButton_Click(object sender, EventArgs e)
    {
        path = Server.MapPath(Uploader.FileName);
        //Label1.Text = path;
        path = path.Replace(@"\", @"-");
        path=path.Substring(0,path.LastIndexOf('-')+1);
        path = path.Replace(@"-", @"\\");
        //Label1.Text = path;
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack)
        {
            string path = "E:\\DOS\\Projects\\Proj 4\\DistributedDebugger\\Logs\\fwd\\";
            foreach (String file in Directory.EnumerateFiles(path, "*.log"))
            {
                string[] arr = file.Split('\\');
                DropDownList1.Items.Add(arr[7]);
            }  
        }
    }
}