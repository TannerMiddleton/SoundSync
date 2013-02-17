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
             bool continueApp = true;

             while (continueApp == true)
             {
                 Program sync = new Program();

                 if (Settings.Default.userSaved != "")
                 {
                     sync.colortext("MSG: Do you want to use [ " + Settings.Default.userSaved + " ] as the user? [Y/N]", "green");
                     string answer = Console.ReadLine();

                     if (answer.ToLower() == "n")
                     {
                         sync.setupConf();

                         sync.downloadSongs();
                     }
                     else if (answer.ToLower() == "y")
                     {
                         sync.downloadSongs();
                     }
                 }
                 else if (Settings.Default.userSaved == "")
                 {
                     sync.setupConf();
                     sync.downloadSongs();
                 }

                 sync.colortext("MSG: Would You Like To Continue? [Y/N]", "Green");

                 string loop = Console.ReadLine();

                 if (loop.ToLower() == "n")
                 {
                     continueApp = false;
                 }
             }
        }

        private void colortext(string text, string getcolor)
        {
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), getcolor, true);
            Console.WriteLine(text);
            Console.ResetColor();
        }

        private static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidReStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
            return System.Text.RegularExpressions.Regex.Replace(name, invalidReStr, "_");
        }

        //You can get your API key buy visiting here - http://soundcloud.com/you/apps/ and registering a new Appliation and taking its Client ID
        private string api = "Your Soundcloud API Key Goes Here!";
        private string username = Settings.Default.userSaved;

        private void setupConf()
        {
            colortext("MSG: Please Enter In A Username To Download Tracks From.", "green");

            string basedir2 = Directory.GetCurrentDirectory();

             username = Console.ReadLine();

            Settings.Default.userSaved = username;
            Settings.Default.Save();
        }

        private void downloadSongs()
        {
            try
            {
                string basedir = Directory.GetCurrentDirectory();
                string ResolveUrl = "http://api.soundcloud.com/resolve.xml?url=http://soundcloud.com/" + username + "/&client_id=" + api;

                colortext("MSG: Getting your user id..", "green");
                XmlDocument doc = new XmlDocument();
                doc.Load(ResolveUrl);
                string userid = doc.DocumentElement.SelectSingleNode("id").InnerText;

                colortext("Press [1] To Download All The Users Tracks, Press [2] To Download User Likes", "Green");

                int answer = Convert.ToInt16(Console.ReadLine());
                doc = new XmlDataDocument();

                if (answer == 1)
                {
                    string UserUrl = "http://api.soundcloud.com/users/" + userid + "/tracks.xml?client_id=" + api;
                    doc.Load(UserUrl);
                }
                else if (answer == 2)
                {
                    string UserUrl = "http://api.soundcloud.com/users/" + userid + "/favorites.xml?client_id=" + api;
                    doc.Load(UserUrl);
                }  

                colortext("MSG: Getting downloadable tracks..", "green");
                Console.WriteLine("-----------------------------------");

                foreach (XmlNode child in doc.DocumentElement)
                {
                    string candownload = child.SelectSingleNode("downloadable").InnerText;

                    if (candownload == "true")
                    {
                        string title = child.SelectSingleNode("title").InnerText;
                        string trackid = child.SelectSingleNode("id").InnerText;
                        string genre = child.SelectSingleNode("genre").InnerText;
                        string artist = child.SelectSingleNode("user").SelectSingleNode("username").InnerText;
                        string downloadurl = child.SelectSingleNode("download-url").InnerText + "?client_id=" + api;
                        string filename = MakeValidFileName(title + ".mp3");
                        string folderstructure = null;
                        if (genre == "") { folderstructure = basedir + @"\Music\UnknownGenre\" + artist; } else { folderstructure = basedir + @"\Music\" + genre + @"\" + artist; }
                        if (!Directory.Exists(folderstructure)) { Directory.CreateDirectory(folderstructure); }
                        if (File.Exists(folderstructure + @"\" + filename))
                        {
                            colortext("File Exists: " + title, "red");
                        }
                        else
                        {
                            WebClient client = new WebClient();
                            colortext("Downloading: " + title, "yellow");
                            Console.SetCursorPosition(0, Console.CursorTop - 1);
                            client.DownloadFile(downloadurl, folderstructure + @"\" + filename);
                            colortext("Complete: " + title + "          ", "green");
                        }
                    }
                }
            }
            catch (Exception)
            {
               colortext("MSG: The Username Is Incorrect." , "Red");

               setupConf();
            }
        }
    }
}


