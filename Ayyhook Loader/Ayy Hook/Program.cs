using System;
using System.Windows.Forms;

namespace Ayy_Hook
//Open brackets
{
    static class Program
    //Open brackets
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        //it speaks for itself ^^^^^^^^^^^^^^^^^^^^^^

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
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
