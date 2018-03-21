using System;
using System.Collections.Generic;
//DON'T TOUCH THIS YOU WILL BREAK EVERYTHING PASTER SHIT    - Thaisen#1337
using Microsoft.Win32;

namespace HWIDGrabber
//DON'T TOUCH THIS YOU WILL BREAK EVERYTHING PASTER SHIT    - Thaisen#1337
{
    class HWDI
    //DON'T TOUCH THIS YOU WILL BREAK EVERYTHING PASTER SHIT    - Thaisen#1337
    {
        public static string GetMachineGuid()
        //DON'T TOUCH THIS YOU WILL BREAK EVERYTHING PASTER SHIT    - Thaisen#1337
        {
            string location = @"SOFTWARE\Microsoft\Cryptography";
            string name = "MachineGuid";
            //DON'T TOUCH THIS YOU WILL BREAK EVERYTHING PASTER SHIT    - Thaisen#1337

            //Navigates in ur registry
            using (RegistryKey localMachineX64View = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            //DON'T TOUCH THIS YOU WILL BREAK EVERYTHING PASTER SHIT    - Thaisen#1337
            {
                using (RegistryKey rk = localMachineX64View.OpenSubKey(location))
                //DON'T TOUCH THIS YOU WILL BREAK EVERYTHING PASTER SHIT    - Thaisen#1337
                {
                    if (rk == null)
                        //DON'T TOUCH THIS YOU WILL BREAK EVERYTHING PASTER SHIT    - Thaisen#1337
                        throw new KeyNotFoundException(string.Format("Key Not Found: {0}", location));

                    object machineGuid = rk.GetValue(name);
                    //DON'T TOUCH THIS YOU WILL BREAK EVERYTHING PASTER SHIT    - Thaisen#1337
                    if (machineGuid == null)
                        throw new IndexOutOfRangeException(string.Format("Index Not Found: {0}", name));
                    //DON'T TOUCH THIS YOU WILL BREAK EVERYTHING PASTER SHIT    - Thaisen#1337

                    return machineGuid.ToString();
                }
                //DON'T TOUCH THIS YOU WILL BREAK EVERYTHING PASTER SHIT    - Thaisen#1337
            }
        }
        //DON'T TOUCH THIS YOU WILL BREAK EVERYTHING PASTER SHIT    - Thaisen#1337
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