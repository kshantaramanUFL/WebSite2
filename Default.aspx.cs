using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

public partial class _Default : System.Web.UI.Page
{
    string actorRef;
    string path = "E:\\DOS\\Projects\\Proj 4\\DistributedDebugger\\Logs\\fwd\\";
    protected void Page_Load(object sender, EventArgs e)
    {
        Label1.Visible = false;
        Label2.Visible = false;
        TextBox1.Visible = false;
        TextBox2.Visible = false;
        if (IsPostBack)
        {
            dropdownitemspopuate(path);
        }
    }
    protected void UploadButton_Click(object sender, EventArgs e)
    {
        //DropDownList1.Visible = true;
        //path = Server.MapPath(Uploader.FileName);
        //Label1.Visible = false;
        //Label2.Visible = false;
        //TextBox1.Visible = false;
        //TextBox2.Visible = false;
        //path = path.Replace(@"\", @"-");
        //path = path.Substring(0, path.LastIndexOf('-') + 1);
        //path = path.Replace(@"-", @"\\");
        //foreach (String file in Directory.EnumerateFiles(path, "*.log"))
        //{
        //    string[] arr = file.Split('\\');
        //    DropDownList1.Items.Add(arr[7]);
        //}
     }
    private void dropdownitemspopuate(string path)
    {
        foreach (String file in Directory.EnumerateFiles(path, "*.log"))
        {
            string[] arr = file.Split('\\');
            DropDownList1.Items.Add(arr[7]);
        }  
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        Label1.Visible = true;
        Label2.Visible = true;
        TextBox1.Visible = true;
        TextBox2.Visible = true;
        String line;
        String TextBoxText ="";
        String recdString = "Received";
        String sentString = "Sending";
        StreamReader file = new StreamReader(path + DropDownList1.SelectedValue);
        string CurrentActorRef = "";
        int sentCounter = 0, recdCounter = 0;
        List<string> reCdList = new List<string>();
        List<string> sentList = new List<string>();
        var recdSet = new HashSet<string>();
        var sentSet = new HashSet<string>();
        while ((line = file.ReadLine()) != null)
        {
            string[] lineContents = line.Split(' ');
            string actorPath = Regex.Match(lineContents[5], @"\[([^]]*)\]").Groups[1].Value;
            actorRef = actorPath.Substring(actorPath.LastIndexOf('/') + 1);
            char[] arr = { '[' };
            if (line.Contains("Minoatour"))
            {
                string CurrentActorPath = Regex.Match(lineContents[8], @"\[([^]]*)\]").Groups[1].Value;
                TextBoxText = "Actor Created, Actor ID : " + CurrentActorPath.Substring(CurrentActorPath.LastIndexOf('/') + 1);
                CurrentActorRef = CurrentActorPath.Substring(CurrentActorPath.LastIndexOf('/') + 1);
            }
            if (line.Contains(recdString))
            {
                TextBoxText = TextBoxText + "\n" + "actor " + CurrentActorRef + "received message from " + actorRef;
                reCdList.Add(actorRef);
                recdSet.Add(actorRef);
                recdCounter = recdCounter + 1;
            }
            if (line.Contains(sentString))
            {
                TextBoxText = TextBoxText + "\n" + "actor " + CurrentActorRef +" sent a message to " + actorRef;
                sentList.Add(actorRef);
                sentSet.Add(actorRef);
                sentCounter = sentCounter + 1;
            }
            TextBox1.Text = TextBoxText + "\n";
       }
        if (sentCounter == recdCounter)
        {
            TextBox2.Text = "Number of Messages Sent : " + sentCounter + "\n" + "Number of Messages Received " + recdCounter + "\n Algorithm Terminated without errors";
        }
        else if (sentCounter != recdCounter)
        {
            String st = checkerror(sentList, reCdList,sentSet);
            TextBox2.Text = "Number of Messages Sent : " + sentCounter + "\n" + "Number of Messages Received " + recdCounter + "\n Error in Program \n " + st;
        }
        DropDownList1.Items.Clear();
        //dropdownitemspopuate(path);
    }

    private string checkerror(List<string> sentList, List<string> reCdList, HashSet<string> sentSet)
    {
        string st = "";
        foreach (String e in sentSet)
        {
         int scount=0, rcount=0;
                foreach (String r in reCdList)
                {
                    if (r.Equals(e))
                    {
                        rcount = rcount + 1;
                    }
                }
                foreach (String s in sentList)
                {
                    if (s.Equals(e))
                    {
                        scount = scount + 1;
                    }
                }
                scount = scount - rcount;
            if(scount>0)
                st = st + "There are "+ scount + " error(s) in " + e + "\n";
            }
            return st;
    }
}