using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Diagnostics;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.IO;
using System.Net;

namespace Plateform_Launcher
{
    public partial class Form1 : Form
    {
        public Form1(string[] args)
        {
            InitializeComponent();

            this.button1.Click += Button1_Click;

            if (args.Length == 1 && args[0].StartsWith("game_exited-"))
            {
                if (DialogResult.No == MessageBox.Show("The game has exited with code " + args[0].Substring(12) + ". Do you want to reopen launcher ?", "Game exited", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    Environment.Exit(0);
                }
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.button1.Enabled = false;
            this.label1.Text = "Status : Check for JRE installation...";
            this.progressBar1.Style = ProgressBarStyle.Marquee;

            if (Environment.GetEnvironmentVariable("JAVA") != null)
            {
                this.label1.Text = "Status : Downloading Game Runnable...";
                this.progressBar1.Style = ProgressBarStyle.Continuous;
                this.progressBar1.Style = ProgressBarStyle.Marquee;

                new Thread(() =>
                {
                    DownloadFromGithub("https://api.github.com/repos/Plateform-Game/Plateform/contents/bin?ref=master", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PlateformLauncher\\Game\\bin");
                    new WebClient().DownloadFile("https://github.com/Plateform-Game/Plateform/raw/master/resources/json.jar", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PlateformLauncher\\Game\\bin\\json.jar");
                }).Start();


                this.label1.Text = "Status : Running...";

                Thread th = new Thread(new ParameterizedThreadStart((object launcherStillOpen) => 
                {
                    Duet<bool, Form1> lso = (Duet<bool, Form1>)launcherStillOpen;
                    ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", "/c \"java -Dfile.encoding=UTF-8 -classpath \\\"" + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PlateformLauncher\\Game\\bin;" + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PlateformLauncher\\Game\\bin\\json.jar\\\" fr.julman.startup.Main\"")
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true
                    };
                    Process game = Process.Start(psi);
                    game.WaitForExit();
                    if (lso.Key)
                    {
                        Process launcher = Process.Start(Application.ExecutablePath, "game_exited-"+game.ExitCode);
                    }
                }));
                th.Start(new Duet<bool, Form1>(!this.checkBox1.Checked, this));
                if (!this.checkBox1.Checked)
                {
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Error: No JRE found ! Must install a new or restart your computer.\n\nCode: ∅", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.button1.Enabled = true;
                this.label1.Text = "Status :";
                this.progressBar1.Style = ProgressBarStyle.Continuous;
            }
        }

        private void DownloadFromGithub(string apiUrl, string folderOutput)
        {
            new Thread(new ThreadStart(() =>
            {
                try
                {
                    RestClient client = new RestClient(apiUrl);
                    RestResponse res = (RestResponse)client.Execute(new RestRequest(Method.GET));

                    JArray array = (JArray)JsonConvert.DeserializeObject(res.Content);

                    foreach (var obj in array)
                    {
                        JObject file = (JObject)obj;
                        string download = file.Value<string>("download_url");
                        if (download == null)
                        {
                            DownloadFromGithub(file.Value<string>("url"), folderOutput + "\\" + file.Value<string>("name"));
                        }
                        else
                        {
                            Directory.CreateDirectory(folderOutput);
                            new WebClient().DownloadFile(file.Value<string>("download_url"), folderOutput + "\\" + file.Value<string>("name"));
                        }
                    }
                }
                catch
                {
                }
            })).Start();
        }

    }
}
