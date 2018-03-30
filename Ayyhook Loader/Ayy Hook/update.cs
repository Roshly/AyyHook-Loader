using System;
using System.Windows.Forms;
using System.Net;
using System.Threading;

//Why do I get the feeling you are going to be using this loader for a p2c? :thinking:


namespace Ayy_Hook
{
    public partial class update : MetroFramework.Forms.MetroForm
    {
        public update()
        {
            InitializeComponent();
        }

        string versioncheck = new WebClient().DownloadString(Settings.Check); //Gets the value of your version.txt

        string location = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

        private void update_Load(object sender, EventArgs e)
        {
            //Just a littly shitty animation I made cause y not
            metroLabel1.Text = "Loading.";
            Thread.Sleep(100);
            metroLabel1.Text = "Loading..";
            Thread.Sleep(100);
            metroLabel1.Text = "Loading...";
            Thread.Sleep(100);
            metroLabel1.Text = "Loading.";
            Thread.Sleep(100);
            metroLabel1.Text = "Loading..";
            Thread.Sleep(100);
            metroLabel1.Text = "Loading...";
            Thread.Sleep(100);
            metroLabel1.Text = "Checking for update";
            //End of animation

            if (versioncheck != Settings.version.ToString())
            {
                timer1.Start();
            }
            else
            {
                timer2.Start();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            metroLabel1.Text = "Update found!";
            Thread.Sleep(100);
            metroLabel1.Text = "Downloading";
            WebClient webClient = new WebClient(); //The thing used to connect to the worst place in the world...! The INTERNET!!! *DUN DUN DUNNNNNNN*
            webClient.DownloadFile(Settings.Update, location); //Code to download "your" loader
            metroLabel1.Text = "Done!";
            Application.Exit(); //Close the loader after it downloaded the new loader
            timer1.Stop();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            metroLabel1.Text = "No update found :)";
            Thread.Sleep(1000);
            //All this does is just closes this form and opens form2. so lemme break it down:
            //Makes a variable called form2 which equals to open form2
            var form1 = new Form1();
            //Overall its just a tricky way to not have to declare an entire function (event handler) 
            //outside of the current one that handles Form2.Closed event. – KDecker 
            //Also note: I didnt really know how to explain this but this guy did a good job!
            form1.Closed += (s, args) => this.Close();
            //This shows form2
            form1.Show();
            //This closes/hides this form
            this.Hide();
            timer2.Stop();
        }
    }
}

//-----------------------------------------------------
// Coded by /id/Roshly! Ayyhook loader source
// https://github.com/Roshly/AyyHook-Loader
// Note to the person using this, removing this
// text is in violation of the license you agreed
// to by downloading. Only you can see this so what
// does it matter anyways.
// Copyright © Roshly 2018
// Licensed under a MIT license
// Read the terms of the license here
// https://github.com/Roshly/AyyHook-Loader/blob/master/LICENSE
//-----------------------------------------------------
