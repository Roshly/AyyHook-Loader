//Things you use go here!
//Condoms dont
using System;
using HWIDGrabber;
using System.Windows.Forms;

//Just something to point out (even tho u dont see it) it took me more time to center the "//Close brackets" then to add them

//Also... Why do i get the feeling that theres more comments then actuall code? :thinking:

namespace Ayy_Hook
//Open brackets
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    //Open brackets
    {
        //Save a phrase hwid so it can hold a string or letters and numbers in it
        string hwid;

        //This is the load, its auto generated but visual studio and tbh theres really nothing much i can explain
        public Form1()
        //Open brackets
        {
            InitializeComponent();
        //Close brackets
        }

        //Basicly as soon as you enter form1 it runs this code first then the rest
        private void Form1_Load(object sender, EventArgs e)
        //Open brackets
        {
            //Basicly make hwid your hwid without the long string (which is found after the equals sign)
            hwid = HWDI.GetMachineGuid();


            if (Properties.Settings.Default.Checked == true) //If the checkbox was set true from last launch:
            //Open brackets
            {
                metroTextBox1.Text = Properties.Settings.Default.Username; //Fill-in last username
                metroTextBox2.Text = Properties.Settings.Default.Password; //Fill-in last username
                metroCheckBox1.Checked = Properties.Settings.Default.Checked; //Check the checkbox
            //Close brackets
            }
        //Close brackets
        }

        //As i was adding comments i relized that i couldve named the buttons and labels what they are supposed to be
        //Instead of leaving it as metro label but... Nah... Wayyyyyyy tooooo much work :P
        private void metroButton1_Click(object sender, EventArgs e) //What happenes after you click the login button
        //Open brackets
        {
            Properties.Settings.Default.Username = metroTextBox1.Text; //Saves your username
            Properties.Settings.Default.Password = metroTextBox2.Text; //Saves your password
            Properties.Settings.Default.Checked = metroCheckBox1.Checked; //Saves the checkbox current state (checked or not)
            Properties.Settings.Default.Save(); //Execute the saving

            //This is where the program goes onto yout site with the check.php file in order to check authentication
            //Note: If your woundering where the links are, they are in settings.cs which is shown below as Settings.Auth
            webBrowser1.Navigate(Settings.Auth + "?username=" + metroTextBox1.Text + " & password=" + metroTextBox2.Text + "&hwid=" + hwid);
        //Close brackets
        }

        //This part is the response you get after you visit the site
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        //Open brackets
        {
            //So if the site responds with p1, it means that your password was correct and it can move on, otherwise it will go to the else statement
            if (webBrowser1.DocumentText.Contains("p1"))
            //Open brackets
            {
                //Next we check for your group! As long as you have one of these groups, your all good to go, otherwise it will go to the else statement
                //Note: I said else statement and not if else, they are 2 completly different things 
                //Also the || means and so it will check for all of these groups at once
                if (webBrowser1.DocumentText.Contains("g4") || webBrowser1.DocumentText.Contains("g6") || webBrowser1.DocumentText.Contains("g8"))
                //Open brackets
                {
                    //And now we have gotten to the final part (for checks).
                    //This tells the program if your hwid is correct or not.
                    //h1 means hwid is correct!
                    if (webBrowser1.DocumentText.Contains("h1"))
                    //Open brackets
                    {
                        //All this does is just closes this form and opens form2. so lemme break it down:
                        //Makes a variable called form2 which equals to open form2
                        var form2 = new Form2();
                        //Overall its just a tricky way to not have to declare an entire function (event handler) 
                        //outside of the current one that handles Form2.Closed event. – KDecker 
                        //Also note: I didnt really know how to explain this but this guy did a good job!
                        form2.Closed += (s, args) => this.Close();
                        //This shows form2
                        form2.Show();
                        //This closes/hides this form
                        this.Hide();
                    //Close brackets
                    }
                    //This tells the program that your hwid is incorrect
                    else if (webBrowser1.DocumentText.Contains("h2"))
                    //Open brackets
                    {
                        //So it gives a error message "Error: Incorrect HWID"
                        MetroFramework.MetroMessageBox.Show(this, "Error : Incorrect HWID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, 100);
                    //Close brackets
                    }
                    //This tells the program that your a new user and your hwid has just been set
                    else if (webBrowser1.DocumentText.Contains("h3"))
                    //Open brackets
                    {
                        //So it gives a message saying "Note: Setting new HWID"
                        MetroFramework.MetroMessageBox.Show(this, "Note: Setting new HWID.", "HWID Reset", MessageBoxButtons.OK, MessageBoxIcon.Error, 100);
                    //Close brackets
                    }
                //Close brackets
                }
                //Now... Since your group didnt match up, we end up here... at this other... else... statement... (what am i doing with my life *sigh*)
                else
                //Open brackets
                {
                    //So it gives a error message saying "Error : Incorrect group"
                    MetroFramework.MetroMessageBox.Show(this, "Error : Incorrect group.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, 100);
                //Close brackets
                }
            //Close brackets
            }
            //And if all else fails, we end up here
            else
            //Open brackets
            {
                //And so it FINALLY gives us this error message saying "Error : Incorrect username or password."
                MetroFramework.MetroMessageBox.Show(this, "Error : Incorrect username or password.", "HWID Reset", MessageBoxButtons.OK, MessageBoxIcon.Error, 100);
            //Close brackets
            }
        //Close brackets
        }

        private void metroTextBox1_KeyDown(object sender, KeyEventArgs e) //When you press a curtain key down, execute code:
        //Open brackets
        {
            if (e.KeyCode == Keys.Enter) //If you pressed enter in the text box:
            //Open brackets
            {
                metroButton1_Click(this, new EventArgs()); //Activate the login button (aka execute login button code)
            //Close brackets
            }
        //Close brackets
        }

        private void metroTextBox2_KeyDown(object sender, KeyEventArgs e) //When you press a curtain key down, execute code:
        //Open brackets
        {
            if (e.KeyCode == Keys.Enter) //If you pressed enter in the text box:
            //Open brackets
            {
                metroButton1_Click(this, new EventArgs()); //Activate the login button (aka execute login button code)
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