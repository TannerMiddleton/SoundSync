using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Soundcloud.Properties;

namespace Soundcloud
{
    class Program
    {
        static void Main(string[] args)
        {
            Program sync = new Program();
            DownloadSongs download = new DownloadSongs();

            bool continueApp = true;

            while (continueApp == true)
            {
                if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() == true)
                {
                    if (Settings.Default.userSaved != "")
                    {
                        sync.colortext("MSG: Do you want to use [ " + Settings.Default.userSaved + " ] as the user? [Y/N]", "green");
                        string answer = Console.ReadLine();

                        if (answer.ToLower() == "n")
                        {
                            sync.setupConf();

                            download.downloadSongs();
                        }
                        else if (answer.ToLower() == "y")
                        {
                            download.downloadSongs();
                        }
                    }
                    else if (Settings.Default.userSaved == "")
                    {
                        sync.setupConf();
                        download.downloadSongs();
                    }

                    sync.colortext("MSG: Would You Like To Continue? [Y/N]", "Green");

                    string loop = Console.ReadLine();

                    if (loop.ToLower() == "n")
                    {
                        continueApp = false;
                    }
                }
                else if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() == false)
                {
                    sync.colortext("MSG: No Internet Connection Available!", "Red");
                    continueApp = false;
                }
            }
        }

        public void colortext(string text, string getcolor)
        {
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), getcolor, true);
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public void setupConf()
        {
            colortext("MSG: Please Enter In A Username To Download Tracks From.", "green");

            string basedir2 = Directory.GetCurrentDirectory();

            Settings.Default.userSaved = Console.ReadLine();
            Settings.Default.Save();
        }

    }
}

