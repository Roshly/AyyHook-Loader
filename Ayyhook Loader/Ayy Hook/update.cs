// uses the the System namespace
using System;
// uses the System.Windows.Forms namespace
using System.Windows.Forms;
// uses the System.Net namespace
using System.Net;
// uses the System.Threading namespace
using System.Threading;
//(The using comments were given by Peatreat)

//Why do I get the feeling you are going to be using this loader for a p2c? :thinking:

//Also im too lazy to comment the rest of this loader so feel free to add your own and make a fork and ill merge it

namespace Ayy_Hook
//Open brackets
{
    public partial class update : MetroFramework.Forms.MetroForm
    //Open brackets
    {
        public update()
        //Open brackets
        {
            InitializeComponent(); //Kys
        //Close brackets
        }

        string versioncheck = new WebClient().DownloadString(Settings.Check); //Gets the value of your version.txt and saves it as a string named versioncheck

        string location = System.IO.Path.GetDirectoryName(Application.ExecutablePath); //Gets the current dir/location of the loader and saves it as a string named location

        private void update_Load(object sender, EventArgs e) //The code that is executed as soon as u open this form up
        //Open brackets
        {
            //Just a littly shitty animation I made cause y not
            metroLabel1.Text = "Loading."; //Changes the label to say "Loading."
            //Thread.Sleep means it will take a quick nap
            //I SWEAR TO FUCKING GOD IF YOU ASK ME WTF IS A NAP I WILL BAN YOU FROM THE DISCORD
            //Note: every 1000 = 1 second
            Thread.Sleep(100);
            metroLabel1.Text = "Loading.."; //Changes the label to say "Loading.."
            Thread.Sleep(100); //Thread.Sleep means it will take a quick nap
            metroLabel1.Text = "Loading..."; //Changes the label to say "Loading..."
            Thread.Sleep(100); //Thread.Sleep means it will take a quick nap
            metroLabel1.Text = "Loading."; //Changes the label to say "Loading."
            Thread.Sleep(100); //Thread.Sleep means it will take a quick nap
            metroLabel1.Text = "Loading.."; //Changes the label to say "Loading.."
            Thread.Sleep(100); //Thread.Sleep means it will take a quick nap
            metroLabel1.Text = "Loading..."; //Changes the label to say "Loading..."
            Thread.Sleep(100); //Thread.Sleep means it will take a quick nap
            metroLabel1.Text = "Checking for update"; //Changes the label to say "Checking for update"
            //End of animation
            //Funfact: that small shitty animation I did plays too fast so u dont even see it when u open the loader

            if (versioncheck != Settings.version.ToString()) //Checks if url string is not equal to string in memory. -Peatreat
            //Open brackets
            {
                timer1.Start(); //Starts timer1 so look at the code down below
            //Close brackets
            }
            else //If its not equal:
            //Open brackets
            {
                timer2.Start(); //Starts timer2 so look at the code down below
            //Close brackets
            }
        //Close brackets
        }

        private void timer1_Tick(object sender, EventArgs e) //This is timer1 and if it was activated, well... code is below, this is self explanitory so kys
        //Open brackets
        {
            metroLabel1.Text = "Update found!"; //Changes the label to say "Update found!"
            Thread.Sleep(100);//Thread.Sleep means it will take a quick nap
            metroLabel1.Text = "Downloading"; //Changes the label to say "Downloading"
            WebClient webClient = new WebClient(); //The thing used to connect to the worst place in the world...! The INTERNET!!! *DUN DUN DUNNNNNNN*
            webClient.DownloadFile(Settings.Update, location); //Code to download "your" loader
            metroLabel1.Text = "Done!"; //Changes the label to say "Done!"
            Application.Exit(); //Close the loader after it downloaded the new loader
            timer1.Stop(); //Stops the timer so this code ends and does not repeat. If you dont undertstand this, comment out this entire line and find out.
        //Close brackets
        }

        private void timer2_Tick(object sender, EventArgs e)
        //Open brackets
        {
            metroLabel1.Text = "No update found :)"; //Changes the label to say "No update found :)"
            Thread.Sleep(1000); //Thread.Sleep means it will take a quick nap for 1 sec (Litterly)
            //---------------------------------------------------------------------------------------
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
            //---------------------------------------------------------------------------------------
            timer2.Stop();//Stops the timer so this code ends and does not repeat. If you dont undertstand this, comment out this entire line and find out.
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
