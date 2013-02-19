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

        public void changePath()
        {
            Program sync = new Program();
            sync.colortext("MSG: Would Like To Change Your Download Path? [Y/N]", "Yellow");
            sync.colortext("MSG: The Current Directory Is [ " + Settings.Default.chooseDirectory + " ]", "Red");
            string path = Console.ReadLine();

            if (path.ToLower() == "y")
            {
                Settings.Default.savedDirectory = false;
                Settings.Default.Save();
            }
            else if (path.ToLower() == "n")
            {
                Settings.Default.savedDirectory = true;
                Settings.Default.Save();
            }

            if (Settings.Default.savedDirectory == false)
            {
                sync.colortext("MSG: Please Paste The Directory Path You Want To Save Music To Below", "Yellow");

                Settings.Default.chooseDirectory = Console.ReadLine(); //Directory.GetCurrentDirectory();
                Settings.Default.savedDirectory = true;
                Settings.Default.Save();
            }
        }

        private int answer;

        public void downloadSongs()
        {
            Program sync = new Program();

            try
            {
                string ResolveUrl = "http://api.soundcloud.com/resolve.xml?url=http://soundcloud.com/" + Settings.Default.userSaved + "/&client_id=" + Settings.Default.apiKey;

                sync.colortext("MSG: Getting The Users ID..", "green");
                XmlDocument doc = new XmlDocument();
                doc.Load(ResolveUrl);
                string userid = doc.DocumentElement.SelectSingleNode("id").InnerText;

                sync.colortext("Press [1] To Download All The Users Tracks, Press [2] To Download User Likes, Or Press [3] To Download All The Tracks Of Everyone You Follow.", "Green");

                answer = Convert.ToInt16(Console.ReadLine());
                doc = new XmlDataDocument();

                if (answer == 1)
                {
                    string UserUrl = "http://api.soundcloud.com/users/" + userid + "/tracks.xml?client_id=" + Settings.Default.apiKey;
                    doc.Load(UserUrl);

                    downloadThings();
                }
                else if (answer == 2)
                {
                    string UserUrl = "http://api.soundcloud.com/users/" + userid + "/favorites.xml?client_id=" + Settings.Default.apiKey;
                    doc.Load(UserUrl);

                    downloadThings();
                }
                else if (answer == 3)
                {
                    string UserUrlFollowings = "http://api.soundcloud.com/users/" + userid + "/followings.xml?client_id=" + Settings.Default.apiKey;
                    doc.Load(UserUrlFollowings);

                    sync.colortext("MSG: Getting downloadable tracks..", "green");
                    Console.WriteLine("-----------------------------------");

                    foreach (XmlNode child2 in doc.DocumentElement)
                    {

                        XmlDocument doc2 = new XmlDocument();
                        string trackid = child2.SelectSingleNode("id").InnerText;
                        string UserUrl = "http://api.soundcloud.com/users/" + trackid + "/tracks.xml?client_id=" + Settings.Default.apiKey;
                        doc2.Load(UserUrl);

                        foreach (XmlNode child in doc2.DocumentElement)
                        {
                            string candownload = child.SelectSingleNode("downloadable").InnerText;

                            if (candownload == "true")
                            {
                                string title = child.SelectSingleNode("title").InnerText;
                                string trackid2 = child.SelectSingleNode("id").InnerText;
                                string genre = child.SelectSingleNode("genre").InnerText;
                                string artist = child.SelectSingleNode("user").SelectSingleNode("username").InnerText;
                                string downloadurl = child.SelectSingleNode("download-url").InnerText + "?client_id=" + Settings.Default.apiKey;
                                string filename = MakeValidFileName(title + ".mp3");
                                string folderstructure = null;
                                folderstructure = Settings.Default.chooseDirectory + @"\SoundCloudMusic\" + artist;
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
                }
            }
            catch (Exception e)
            {
                if (answer != 1 || answer != 2 || answer != 3)
                {
                    sync.colortext("MSG: Please Enter In A Valid Option.", "Red");
                }
                else if (answer == 1 || answer == 2 || answer == 3)
                {
                    sync.colortext(e.ToString(), "Red");
                }
            }
        }


        public void downloadThings()
        {
            Program sync = new Program();

            XmlDocument doc = new XmlDocument();
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
                    folderstructure = Settings.Default.chooseDirectory + @"\SoundCloudMusic\" + artist;
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
    }
}

