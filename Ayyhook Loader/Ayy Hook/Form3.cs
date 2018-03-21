//im not exactly sure what it does but it makes the code work
//Comment by Ca3sper "be glad I aint usin your bitch too cause we both can see the code and know i aint usin no condoms"
using System;
using System.Linq;
using System.Windows.Forms;
using ManualMapInjection.Injection;
using System.IO;
using System.Diagnostics;
using System.Threading;


namespace Ayy_Hook
//Open brackets
{
    public partial class Form3 : MetroFramework.Forms.MetroForm
    //Open brackets
    {
        //This right here is a bool which is a string named "csgo" that holds eaither a true or false
        bool csgo;

        //This is the load, its auto generated but visual studio and tbh theres really nothing much i can explain
        public Form3()
        //Open brackets
        {
            InitializeComponent();
        //Close brackets
        }

        //Basicly as soon as you enter form3 it runs this code first then the rest
        private void Form3_Load(object sender, EventArgs e)

        //Open brackets
        {
            //Thread.Sleep means it will take a quick nap
            //I SWEAR TO FUCKING GOD IF YOU ASK ME WTF IS A NAP I WILL BAN YOU FROM THE DISCORD
            //Note: every 1000 = 1 second
            Thread.Sleep(200);

            //var means variable so in this case it makes a variable called name and name equals the text csgo
            var name = "csgo";

            //Injection shit using the name var to find proccess
            var target = Process.GetProcessesByName(name).FirstOrDefault();
            //An if statement to check if the dll for ur paste downloaded or not.
            if (File.Exists(Settings.Save))
            //Open brackets
            {
                //Check if file actually downloaded cause most people were complaining about
                //How they see the file, post a pic and i see a 0kb file like wut???
                if (new FileInfo(Settings.Save).Length != 0)
                //Open brackets (Almost missed this one)
                {

                    //If it did download, it sets the bool csgo eaither true or false
                    //In this case (if it did successfully download the dll) it sets it to true
                    csgo = true;
                //Close brackets
                }

                //Literally as the statment says, it means else / or else
                else

                //Open brackets
                {
                    //But in this case (if it did NOT successfully download the dll) it sets it to false
                    csgo = false;
                //Close brackets
                }
            //Close brackets
            }
        //Close brackets
        }

        private void timer1_Tick(object sender, EventArgs e)
        //Open brackets
        {
            if (csgo) //If your shitty paste downloaded, then \/
            //Open brackets
            {
                //Sets the variable name equal to csgo (ment for injection)
                var name = "csgo";
                //Gets the proccess info (csgo) to inject to
                var target = Process.GetProcessesByName(name).FirstOrDefault();
                //If proccess doesnt exist
                if (target != null)
                //Open brackets
                {
                    //More variables but this one is for the dll path
                    var path = (Settings.Save);
                    //And this one is to read the files bytes in order to inject
                    var file = File.ReadAllBytes(path);
                                               //^^^^// Just pointing out the path var from above

                    Thread.Sleep(1000);//Read lines 30-32
                    var injector = new ManualMapInjector(target) { AsyncInjection = true }; //Basicly all paramaters needed in injection as a variable called injector
                    //The actual injection line but Thaisen added this to a label and idk why a label but i guess it just is
                    label1.Text = $"hmodule = 0x{injector.Inject(file).ToInt64():x8}";
                    label1.Text = "Injection complete"; //Thaisen added this line for some reason :thinking:
                    timer1.Stop(); //Stops timer1
                    timer2.Start(); //Starts timer2 (so all timer2 code gets activated)
                //Close brackets
                }
            //Close brackets
            }
            
            else if (!csgo) //If you shitty paste didnt download successfully 
            //Open brackets
            {
                //A error message that says "Error Downloading dll!"
                MetroFramework.MetroMessageBox.Show(this, "Error Downloading dll!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, 100);
                Application.Exit(); //Closes the Loader
            //Close brackets
            }
        //Close brackets
        }

        private void timer2_Tick(object sender, EventArgs e)
        //Open brackets
        {
            File.Delete(Settings.Save); //Deletes the dll after injection
            Application.Exit(); //Closes the Loader
            timer2.Stop(); //Stops the timer (aka the thing that was enabled to trigger this code)
        //Close brackets
        }
    //Close brackets
    }
//Close brackets (Final one, Phewf!)
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