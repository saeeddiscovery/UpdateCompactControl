using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Net.NetworkInformation;
using System.ComponentModel;
using System.Threading;
using System.Net;
using System.Diagnostics;
using System.Net;
using System.Globalization;
using System.IO;

namespace UpdateCompactControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private bool isConnected = false;
        public static bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return pingable;
        }

        private void CheckConnection()
        {
            lbl_Connection.Content = "---";
            grid_Progress.Visibility = Visibility.Hidden;
            if (PingHost("8.8.8.8") == false)
            {
                lbl_Connection.Content = "Not Connected";
                lbl_Connection.Foreground = Brushes.Orange;
                isConnected = false;
            }
            else
            {
                lbl_Connection.Content = "Connected";
                lbl_Connection.Foreground = Brushes.Green;
                isConnected = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            checkEveryThing();
        }

        private void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            checkEveryThing();
        }

        private void checkEveryThing()
        {
            System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            CheckConnection();
            CheckVersions();
            System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
        }

        private string checkVersion_local()
        {
            string localVer = "";

            try
            {
                string pathToExe = "CompactControl.exe";
                var versionInfo = FileVersionInfo.GetVersionInfo(pathToExe);
                // localVer = versionInfo.FileVersion;

                localVer = versionInfo.FileMajorPart + "." +
                    versionInfo.FileMinorPart + "." +
                    versionInfo.FileBuildPart;
            }
            catch (Exception)
            {
                localVer = "";
                MessageBox.Show("Could not check the version of your CompactControl!",
                    "Something went wrong!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return localVer;
        }

        private string latestVersionFull = "";
        private string checkVersion_online()
        {
            string onlineVer = "";

            try
            {
                WebRequest request = WebRequest.Create("https://github.com/saeeddiscovery/CompactControl/releases/latest");
                using (WebResponse response = request.GetResponse())
                {
                    var uri = response.ResponseUri.AbsolutePath;
                    latestVersionFull = response.ResponseUri.AbsoluteUri.ToString();
                    string[] _onlineVer = uri.Split('/').Last().Split('-').First().Split('.');
                    onlineVer = _onlineVer[0].Substring(1) + '.' + _onlineVer[1] + '.' + _onlineVer[2];
                }
            }
            catch (Exception)
            {
                onlineVer = "";
                MessageBox.Show("Could not check the online version of the CompactControl!",
                    "Something went wrong!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return onlineVer;
        }

        private void CheckVersions()
        {
            lbl_LocalVersion.Content = "---";
            lbl_onlineVersion.Content = "---";
            
            string localVer = checkVersion_local();
            string onlineVer = checkVersion_online();

            if (localVer != "")
                lbl_LocalVersion.Content = localVer;
            if (onlineVer != "")
                lbl_onlineVersion.Content = onlineVer;

            if (localVer == onlineVer)
                btn_Update.IsEnabled = false;
            else
                btn_Update.IsEnabled = true;
        }

        private void startDownload()
        {
            lbl_Download.Content = "Download In Process...";
            btn_Update.IsEnabled = false;
            Thread thread = new Thread(() => {
                WebClient client = new WebClient();

            //WebRequest request = WebRequest.Create("https://github.com/saeeddiscovery/CompactControl/releases/download/v2.1.2-rc.1/CompactControl.zip");
                //using (WebResponse response = request.GetResponse())
                //{
                //    var uri = response.ResponseUri;
                //    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                //    client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                //    client.DownloadFileAsync(uri, @"CompactControl.zip");
                //}

                string urlLatest = latestVersionFull + "/CompactControl.zip";
                urlLatest = urlLatest.Replace("tag", "download");
                Uri myUrl = new Uri(urlLatest);
                //client.Credentials = new NetworkCredential("Userid", "mypassword");
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                client.DownloadFileAsync(myUrl, @"CompactControl.zip");


            });
            thread.Start();
        }
        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)delegate {
                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = bytesIn / totalBytes * 100;
                lbl_Download.Content = "Downloaded " + e.BytesReceived/1024 + " of " + e.TotalBytesToReceive/1024 + " KBs";
                progress_Download.Value = int.Parse(Math.Truncate(percentage).ToString());
            });
        }
        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)delegate {
                lbl_Download.Content = "Completed";
            });
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            Application curApp = Application.Current;
            curApp.Shutdown();
        }

        private void btn_Update_Click(object sender, RoutedEventArgs e)
        {
            grid_Progress.Visibility = Visibility.Visible;
            startDownload();
        }

    }
}
