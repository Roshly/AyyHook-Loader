//using shitty paster's dll
using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.IO;

namespace Ayy_Hook
//Open brackets
{
    public partial class Form2 : MetroFramework.Forms.MetroForm
    //Open brackets
    {
        //This is the load, its auto generated but visual studio and tbh theres really nothing much i can explain
        public Form2()
        //Open brackets
        {
            InitializeComponent();
        //Close brackets
        }

        private void Form2_Load(object sender, EventArgs e)
        //Open brackets
        {
            //Ok so lemme quickly summaraze what this right here does
            //Basicly it adds the cheats to the drop down (when selecting)

            //This adds The first cheat 
            metroComboBox1.Items.Add("CSGO #1");

            //And this adds the second one
            metroComboBox1.Items.Add("CSGO #2");

            //To add more, simply paste another one of that ^ and add another selectedindex and ur good to go (remember to change download link)

        //Close brackets
        }

        //If you dont know what this means \/ i will hunt you down and shoot you! (Hint: IT SAYS FUCKING CLICK!)
        private void metroButton1_Click(object sender, EventArgs e)
        //Open brackets
        {
            if (metroComboBox1.SelectedIndex == -1) // If u didnt select anything, its counted as -1 so we just give a error msg
            //Open brackets
            {
                //Since you didnt select anything, it gives you a error message saying "You must select a cheat to inject!
                MetroFramework.MetroMessageBox.Show(this, "You must select a cheat to inject!", "", MessageBoxButtons.OK, MessageBoxIcon.Error, 100);
            //Close brackets
            }

            if (metroComboBox1.SelectedIndex == 0) //If you selected the first option(which is counted as 0) then execute this code:
            //Open brackets
            {
                string path = Settings.Pathy; //Set a string path to ur 
                if (!Directory.Exists(path)) //IF directory where your dll is supposed to download doesnt exist, execute code in brackets
                //Open brackets
                {
                    DirectoryInfo di = Directory.CreateDirectory(path); //Make the folder (Cause apperently it doesnt exist)
                    di.Attributes = FileAttributes.Directory | FileAttributes.Hidden; //Make the folder hidden
                //Close brackets
                }
                WebClient wb = new WebClient(); //The thing that connects to the internet / makes the connection
                wb.Headers.Add("User-Agent", Settings.UserAgentString); //Useragent is what the server sees you connecting with
                wb.DownloadFile(Settings.CSGO, Settings.Save); //Downloads the dll from settings.cs file and string called CSGO

                var Form3 = new Form3(); //Makes a variable called form2 which equals to open form3
                Form3.Closed += (s, args) => this.Close(); //Check form1 for explanation
                this.Hide(); //This closes/hides this form
                Form3.Show(); //This shows form3
            //Close brackets
            }

            if (metroComboBox1.SelectedIndex == 1) //If you selected the second option(which is counted as 1) then execute this code
            //Open brackets
            {
                string path = Settings.Pathy; //Set a string path to ur 
                if (!Directory.Exists(path)) //IF directory where your dll is supposed to download doesnt exist, execute code in brackets
                //Open brackets
                {
                    DirectoryInfo di = Directory.CreateDirectory(path); //Make the folder (Cause apperently it doesnt exist)
                    di.Attributes = FileAttributes.Directory | FileAttributes.Hidden; //Make the folder hidden
                //Close brackets
                }
                WebClient wb = new WebClient(); //The thing that connects to the internet / makes the connection
                wb.Headers.Add("User-Agent", Settings.UserAgentString); //Useragent is what the server sees you connecting with
                wb.DownloadFile(Settings.CSGOI, Settings.Save); //Downloads the dll from settings.cs file and string called CSGOI <--- Notice how its different

                var Form3 = new Form3(); //Makes a variable called form2 which equals to open form3
                Form3.Closed += (s, args) => this.Close(); //Check form1 for explanation
                this.Hide(); //This closes/hides this form
                Form3.Show(); //This shows form3
            //Close brackets
            }
        //Close brackets
        }

        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        //Open brackets
        {
            if (metroComboBox1.SelectedIndex == 0) //If the first tab was selected:
            //Open brackets
            {
                Process[] pname = Process.GetProcessesByName("csgo"); //Get the csgo proccess 
                if (pname.Length == 0) //Check if pid is equal to 0
                    //If it is = to 0, game isnt running so display error msg "CSGO Game Process Not Running"
                    MetroFramework.MetroMessageBox.Show(this, "CSGO Game Process Not Running", "", MessageBoxButtons.OK, MessageBoxIcon.Error, 100);
            //Close brackets
            }

            if (metroComboBox1.SelectedIndex == 1)//If the second tab was selected:
            //Open brackets
            {
                Process[] pname = Process.GetProcessesByName("csgo"); //Get the csgo proccess 
                if (pname.Length == 0) //Check if pid is equal to 0
                    //If it is = to 0, game isnt running so display error msg "CSGO Game Process Not Running"
                    MetroFramework.MetroMessageBox.Show(this, "CSGO Game Process Not Running", "", MessageBoxButtons.OK, MessageBoxIcon.Error, 100);
            //Close brackets
            }
        //Close brackets
        }
    //Close brackets
    }
//Close brackets
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