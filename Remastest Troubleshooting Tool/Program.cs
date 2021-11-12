using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Remastest_Troubleshooting_Tool
{
    class Program
    {
        private static List<string> Diagnostics = new List<string>();

        private static string UserProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        public static string ExeDir = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        static void Main(string[] args)
        {
#if DEBUG
            ExeDir = @"F:\Dark Souls Prepare To Die Edition\DATA\";
#endif
            Diagnostics.Add("## OS ##");
            Diagnostics.Add($"{getOSInfo()} {Environment.OSVersion.Version}");
            Diagnostics.Add("");
            Diagnostics.Add("## General: Paths and Files ##");
            var darkSoulsIniFolder = Directory.Exists($@"{UserProfile}\AppData\Local\NBGI\DarkSouls");
            Diagnostics.Add($"Dark Souls Ini Folder Exists = {darkSoulsIniFolder}");

            var dsIniPath = $@"{UserProfile}\AppData\Local\NBGI\DarkSouls\DarkSouls.ini";
            var darkSoulsIni = File.Exists(dsIniPath);
            Diagnostics.Add($"Dark Souls Ini Exists = {darkSoulsIni}");

            var documentsExists = Directory.Exists($@"{UserProfile}\Documents\");
            Diagnostics.Add($"Dark Souls Save Exists = {documentsExists}");

            var darkSoulsSaveFolder = Directory.Exists($@"{UserProfile}\Documents\NBGI\darksouls");
            Diagnostics.Add($"Dark Souls Save Folder Exists = {darkSoulsSaveFolder}");

            var darkSoulsSave = File.Exists($@"{UserProfile}\Documents\NBGI\darksouls\DRAKS0005.sl2");
            Diagnostics.Add($"Dark Souls Save Exists = {darkSoulsSave}");

            var darkSoulsSaveModded = File.Exists($@"{UserProfile}\Documents\NBGI\darksouls\MODSOULS.sl2");
            Diagnostics.Add($"Dark Souls Modded Save Exists = {darkSoulsSaveModded}");

            var darkSoulsRemastered = File.Exists($@"{ExeDir}\DarkSoulsRemastered.exe");
            Diagnostics.Add($"Dark Souls Remastered Exists = {darkSoulsRemastered} (Bad if true)");

            var darkSouls = File.Exists($@"{ExeDir}\DARKSOULS.exe");
            Diagnostics.Add($"Dark Souls Exists = {darkSouls}");

            var modSouls = File.Exists($@"{ExeDir}\MODSOULS.exe");
            Diagnostics.Add($"Dark Souls ModSouls Exists = {modSouls}");

            Diagnostics.Add("");
            Diagnostics.Add("## DarkSouls.ini ##");
            if (darkSoulsIni)
            {
                var dsIni = File.ReadAllLines(dsIniPath);

                foreach (var line in dsIni)
                {
                    if (!line.Contains("#") && DSIniEntries.Any(x => line.Contains(x)))
                        Diagnostics.Add(line);
                }
            }

            Diagnostics.Add("");
            Diagnostics.Add("## DSFix.ini ##");
            var dsFixPath = $@"{ExeDir}\DSFix.ini";
            var dsFixini = File.Exists(dsFixPath);
            if (dsFixini)
            {
                var dsFix = File.ReadAllLines(dsFixPath);

                foreach (var line in dsFix)
                {
                    if (!line.Contains("#") && DSFixEntries.Any(x => line.Contains(x)))
                        Diagnostics.Add(line);
                }
            }

            Diagnostics.Add("");
            Diagnostics.Add("## d3d9_Mod.ini ##");
            var dsOverhaulPath = $@"{ExeDir}\d3d9_Mod.ini";
            var dsOverhaulini = File.Exists(dsOverhaulPath);
            if (dsOverhaulini)
            {
                var dsOverhaul = File.ReadAllLines(dsOverhaulPath);
                foreach (var line in dsOverhaul)
                {
                    if (!line.Contains("#") && DSOverhaulEntries.Any(x => line.Contains(x)))
                        Diagnostics.Add(line);
                }
            }

            File.WriteAllLines($@"{ExeDir}\Diagnostic.txt", Diagnostics);
        }

        //https://stackoverflow.com/questions/2819934/detect-windows-version-in-net
        static string getOSInfo()
        {
            //Get Operating system information.
            OperatingSystem os = Environment.OSVersion;
            //Get version information about the os.
            Version vs = os.Version;

            //Variable to hold our return value
            string operatingSystem = "";

            if (os.Platform == PlatformID.Win32Windows)
            {
                //This is a pre-NT version of Windows
                switch (vs.Minor)
                {
                    case 0:
                        operatingSystem = "95";
                        break;
                    case 10:
                        if (vs.Revision.ToString() == "2222A")
                            operatingSystem = "98SE";
                        else
                            operatingSystem = "98";
                        break;
                    case 90:
                        operatingSystem = "Me";
                        break;
                    default:
                        break;
                }
            }
            else if (os.Platform == PlatformID.Win32NT)
            {
                switch (vs.Major)
                {
                    case 3:
                        operatingSystem = "NT 3.51";
                        break;
                    case 4:
                        operatingSystem = "NT 4.0";
                        break;
                    case 5:
                        if (vs.Minor == 0)
                            operatingSystem = "2000";
                        else
                            operatingSystem = "XP";
                        break;
                    case 6:
                        if (vs.Minor == 0)
                            operatingSystem = "Vista";
                        else if (vs.Minor == 1)
                            operatingSystem = "7";
                        else if (vs.Minor == 2)
                            operatingSystem = "8";
                        else
                            operatingSystem = "8.1";
                        break;
                    case 10:
                        operatingSystem = "10";
                        break;
                    default:
                        break;
                }
            }
            //Make sure we actually got something in our OS check
            //We don't want to just return " Service Pack 2" or " 32-bit"
            //That information is useless without the OS version.
            if (operatingSystem != "")
            {
                //Got something.  Let's prepend "Windows" and get more info.
                operatingSystem = "Windows " + operatingSystem;
                //See if there's a service pack installed.
                if (os.ServicePack != "")
                {
                    //Append it to the OS name.  i.e. "Windows XP Service Pack 3"
                    operatingSystem += " " + os.ServicePack;
                }
                //Append the OS architecture.  i.e. "Windows XP Service Pack 3 32-bit"
                //operatingSystem += " " + getOSArchitecture().ToString() + "-bit";
            }
            //Return the information we've gathered.
            return operatingSystem;
        }

        private static List<string> DSIniEntries = new List<string>()
        {
            "WindowMode",
            "Height",
            "Width",
            "DisplaySettingWindow",
            "DisplaySettingFullScreen",
            "RefreshRate",
            "Antialiasing",
            "Blur"

        };

        private static List<string> DSFixEntries = new List<string>()
        {
            "renderWidth",
            "renderHeight",
            "presentWidth",
            "presentHeight",
            "aaQuality",
            "aaType",
            "unlockFPS",
            "filteringOverride",
            "enableHudMod",
            "borderlessFullscreen",
            "DisplaySettingFilter",
            "disableCursor",
            "captureCursor",
            "enableTextureDumping",
            "enableTextureOverride",
            "forceWindowed",
            "forceFullscreen",
            "enableVsync",
            "fullscreenHz",
            "dinput8dllWrapper",
            "d3dAdapterOverride"

        };

        private static List<string> DSOverhaulEntries = new List<string>()
        {
            "CustomSaveLocation",
            "EnableMultiphantom",
        };

    }
}
