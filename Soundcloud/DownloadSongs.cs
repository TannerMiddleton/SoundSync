using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Soundcloud.Properties;
using System.ComponentModel;
using System.Threading;

namespace Soundcloud
{
    class DownloadSongs
    {

        private static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidReStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
            return System.Text.RegularExpressions.Regex.Replace(name, invalidReStr, "_");
        }

        public void downloadSongs()
        {
            Program sync = new Program();

            try
            {
                string basedir = Directory.GetCurrentDirectory();
                string ResolveUrl = "http://api.soundcloud.com/resolve.xml?url=http://soundcloud.com/" + Settings.Default.userSaved + "/&client_id=" + Settings.Default.apiKey;

                sync.colortext("MSG: Getting The Users ID..", "green");
                XmlDocument doc = new XmlDocument();
                doc.Load(ResolveUrl);
                string userid = doc.DocumentElement.SelectSingleNode("id").InnerText;

                sync.colortext("Press [1] To Download All The Users Tracks, Press [2] To Download User Likes", "Green");

                int answer = Convert.ToInt16(Console.ReadLine());
                doc = new XmlDataDocument();

                if (answer == 1)
                {
                    string UserUrl = "http://api.soundcloud.com/users/" + userid + "/tracks.xml?client_id=" + Settings.Default.apiKey;
                    doc.Load(UserUrl);
                }
                else if (answer == 2)
                {
                    string UserUrl = "http://api.soundcloud.com/users/" + userid + "/favorites.xml?client_id=" + Settings.Default.apiKey;
                    doc.Load(UserUrl);
                }

                sync.colortext("MSG: Getting downloadable tracks..", "green");
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
                        string downloadurl = child.SelectSingleNode("download-url").InnerText + "?client_id=" + Settings.Default.apiKey;
                        string filename = MakeValidFileName(title + ".mp3");
                        string folderstructure = null;
                        folderstructure = basedir + @"\Music\" + artist;
                        if (!Directory.Exists(folderstructure)) { Directory.CreateDirectory(folderstructure); }
                        if (File.Exists(folderstructure + @"\" + filename))
                        {
                            sync.colortext("File Exists: " + title, "red");
                        }
                        else
                        {
                            WebClient client = new WebClient();
                            sync.colortext("Downloading: " + title, "yellow");
                            Console.SetCursorPosition(0, Console.CursorTop - 1);
                            client.DownloadFile(downloadurl, folderstructure + @"\" + filename);
                            sync.colortext("Complete: " + title + "          ", "green");

                        }
                    }
                }
            }
            catch (Exception)
            {
                sync.colortext("MSG: The Username Is Incorrect.", "Red");

                sync.setupConf();
            }
        }
    }
}
