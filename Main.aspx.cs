using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text.RegularExpressions;


public partial class Main : System.Web.UI.Page
{
    string path = "";
    string actorRef;
    int[,] sentArray;
    int[,] recdArray;
    int[,] errorArray;
    Dictionary<string, int> actorKey = new Dictionary<string, int>();

    private void initializeArray(string path)
    {
        int counter = 0;
        foreach (String file in Directory.EnumerateFiles(path, "*.log"))
        {
            string[] arr = file.Split('\\');
            counter = counter + 1;
            StreamReader fileName = new StreamReader(file);
            string actorNumber = arr[7].Substring(0,arr[7].IndexOf('.'));
            int key = Int32.Parse(actorNumber);
            String line;
            string CurrentActorRef;
            while ((line = fileName.ReadLine()) != null)
            {
                string[] lineContents = line.Split(' ');
                if (line.Contains("Minoatour"))
                {
                    string CurrentActorPath = Regex.Match(lineContents[8], @"\[([^]]*)\]").Groups[1].Value;
                    CurrentActorRef = CurrentActorPath.Substring(CurrentActorPath.LastIndexOf('/') + 1);
                    if(!actorKey.ContainsKey(CurrentActorRef))
                    actorKey.Add(CurrentActorRef,key);
                }
            }
        }
        sentArray = new Int32[counter, counter];
        recdArray = new Int32[counter, counter];
        errorArray = new Int32[counter, counter];
        for (int i = 0; i < counter; i++)
        {
            for (int j = 0; j < counter; j++)
            {
                sentArray[i, j] = 0;
                recdArray[i, j] = 0;
                errorArray[i, j] = 0;
            }
        }
    }
    private void dropdownpopulate(string path)
    {
        initializeArray(path);
        foreach (String file in Directory.EnumerateFiles(path, "*.log"))
        {
            string[] arr = file.Split('\\');
            DropDownList1.Items.Add(arr[7]);
            StreamReader fileName = new StreamReader(file);
            String line;
            string CurrentActorRef = "";
            String recdString = "Received";
            String sentString = "Sending";
            while ((line = fileName.ReadLine()) != null)
            {
                string[] lineContents = line.Split(' ');
                string actorPath = Regex.Match(lineContents[5], @"\[([^]]*)\]").Groups[1].Value;
                actorRef = actorPath.Substring(actorPath.LastIndexOf('/') + 1);
                if (line.Contains("Minoatour"))
                {
                    string CurrentActorPath = Regex.Match(lineContents[8], @"\[([^]]*)\]").Groups[1].Value;
                    CurrentActorRef = CurrentActorPath.Substring(CurrentActorPath.LastIndexOf('/') + 1);
                     
                }
                if (line.Contains(recdString))
                {
                    int col = actorKey[actorRef];
                    int row = actorKey[CurrentActorRef];
                    recdArray[row, col] = recdArray[row, col] + 1;
                }
                if (line.Contains(sentString))
                {
                    int col = actorKey[actorRef];
                    int row = actorKey[CurrentActorRef];
                    sentArray[row, col] = sentArray[row, col] + 1;
                }
                for (int i = 0; i < 11; i++)
                {
                    for (int j = 0; j < 11; j++)
                    {
                        int error = (-1) * (recdArray[i, j] - sentArray[j, i]);
                        errorArray[i, j] = error;
                    }
                }
            }
        }
    }

    protected void UploadButton_Click(object sender, EventArgs e)
    {
        if (Uploader.HasFile == true)
        {
            MultiView2.Visible = true;
            MultiView2.ActiveViewIndex = 0;
            Step2.Text = "Step 2 : Choose which File to Debug";
            Button1.Visible = true;
            path = Server.MapPath(Uploader.FileName);
            Label1.Visible = false;
            Label2.Visible = false;
            TextBox1.Visible = false;
            TextBox2.Visible = false;
            path = path.Replace(@"\", @"-");
            path = path.Substring(0, path.LastIndexOf('-') + 1);
            path = path.Replace(@"-", @"\\");
        }
        else
            DropDownList1.Items.Clear();  
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        MultiView1.ActiveViewIndex = 0;
        Step1.Text = " Step 1 : Upload To Server log file directory";
        if (IsPostBack)
        {
            string path = "E:\\DOS\\Projects\\Proj 4\\DistributedDebugger\\Logs\\fwd\\";
            dropdownpopulate(path);
        }
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        MultiView3.Visible = true;
        MultiView3.ActiveViewIndex = 0;
        Step3.Text = "Step 3 : Output of the logs";
        Label1.Visible = true;
        Label2.Visible = true;
        TextBox1.Visible = true;
        TextBox2.Visible = true;
        String line;
        String TextBoxText = "";
        String recdString = "Received";
        String sentString = "Sending";
        string path = "E:\\DOS\\Projects\\Proj 4\\DistributedDebugger\\Logs\\fwd\\";
        StreamReader file = new StreamReader(path + DropDownList1.SelectedValue);
        string CurrentActorRef = "";
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
                TextBoxText = TextBoxText + "\n" + "actor " + CurrentActorRef + "received message from " + actorRef;
            if (line.Contains(sentString))
                TextBoxText = TextBoxText + "\n" + "actor " + CurrentActorRef + " sent a message to " + actorRef;
            TextBox1.Text = TextBoxText + "\n";
        }
            //String st = checkerror(CurrentActorRef);
            TextBox2.Text = checkerror(CurrentActorRef);
            String Path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Visual Studio 2010\WebSites\WebSite2\";
            generateGraph(Path);
            DropDownList1.Items.Clear();   
            dropdownpopulate(path);
   }

    private string checkerror(String actor)
    {
        //TextBox2.Text = "";
        String st = "";
        int errorFlag = 0;
        for (int i = 0; i < 11; i++)
        {
            //TextBox2.Text = TextBox2.Text+errorArray[actorKey[actor], i].ToString() + " ";
            if (errorArray[actorKey[actor], i] > 0)
            {
                errorFlag = 1;
                int scount = errorArray[actorKey[actor], i];
                String e = "";
                foreach (String actorname in actorKey.Keys)
                {
                    if (actorKey[actorname] == i)
                    {
                        e = actorname;
                    }
                }
               st = st + " There are " + scount + " message(s) not recieved from " + e + "\n";
            }
        }
        if(errorFlag==0)
            st="There are no errors";
       return st;
    }
    private void generateGraph(String path)
    {
        FileStream fsent = System.IO.File.Create(path + "\\Sentgraph.gv");
        fsent.Close();
        StreamWriter fSentStream = new StreamWriter(path + "\\Sentgraph.gv");
        FileStream fRecd = System.IO.File.Create(path + "\\Recdgraph.gv");
        fRecd.Close();
        StreamWriter fRecdStream = new StreamWriter(path + "\\Recdgraph.gv");
        FileStream ferror = System.IO.File.Create(path + "\\Errorgraph.gv");
        ferror.Close();
        StreamWriter fErrorStream = new StreamWriter(path + "\\Errorgraph.gv");
        fSentStream.WriteLine("digraph G{");
        fRecdStream.WriteLine("digraph G{");
        fErrorStream.WriteLine("digraph G{");
        for (int i = 0; i < 11; i++)
        {
            for (int j = 0; j < 11; j++)
            {
            
                if(sentArray[i,j] > 0 ) {
                    fSentStream.WriteLine("edge[color=green];");
                    String from = "";
                    foreach (String actorname in actorKey.Keys)
                    {
                        if (actorKey[actorname] == i)
                        {
                            from = actorname;
                        }
                    }
                    from = from.Substring(0, from.LastIndexOf('#'));
                    String to = "";
                    foreach (String actorname in actorKey.Keys)
                    {
                        if (actorKey[actorname] == j)
                        {
                            to = actorname;
                        }
                    }
                    to = to.Substring(0, to.LastIndexOf('#'));
                    fSentStream.WriteLine(from + "->" + to + "[label=\"" + sentArray[i, j] + "\"];");
                }
                if (recdArray[i, j] > 0)
                {
                    fRecdStream.WriteLine("edge[color=blue];");
                    String from = "";
                    foreach (String actorname in actorKey.Keys)
                    {
                        if (actorKey[actorname] == j)
                        {
                            from = actorname;
                        }
                    }
                    from = from.Substring(0, from.LastIndexOf('#'));
                    String to = "";
                    foreach (String actorname in actorKey.Keys)
                    {
                        if (actorKey[actorname] == i)
                        {
                            to = actorname;
                        }
                    }
                    to=to.Substring(0,to.LastIndexOf('#'));
                    fRecdStream.WriteLine(from + "->" + to + "[label=\""+recdArray[i, j]+"\"];");
                }
                if (errorArray[i, j] > 0)
                {
                    fErrorStream.WriteLine("edge[color=red];");
                    String from = "";
                    foreach (String actorname in actorKey.Keys)
                    {
                        if (actorKey[actorname] == j)
                        {
                            from = actorname;
                        }
                    }
                    from = from.Substring(0, from.LastIndexOf('#'));
                    String to = "";
                    foreach (String actorname in actorKey.Keys)
                    {
                        
                        if (actorKey[actorname] == i)
                        {
                            to = actorname;
                        }
                    }
                    to = to.Substring(0, to.LastIndexOf('#'));
                    fErrorStream.WriteLine(from + "->" + to + "[label=\"" + errorArray[i, j] + "\"];");
                }
            }
        }
        fSentStream.WriteLine("}");
        fRecdStream.WriteLine("}");
        fErrorStream.WriteLine("}");
        fSentStream.Close();
        fRecdStream.Close();
        fErrorStream.Close();
        }
    protected void Button2_Click(object sender, EventArgs e)
    {
        System.Diagnostics.Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Visual Studio 2010\WebSites\WebSite2\SentGraph.bat").WaitForExit();
        System.Diagnostics.Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Visual Studio 2010\WebSites\WebSite2\RecdGraph.bat").WaitForExit();
        System.Diagnostics.Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Visual Studio 2010\WebSites\WebSite2\ErrorGraph.bat").WaitForExit();
        Step4.Text = "Graph";
        ImageView.Visible = true;
        ImageView.ActiveViewIndex = 0;
    }
}