using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.NetworkInformation;
using MahApps.Metro.Controls;
using System.ComponentModel;
using System.Threading;
using System.Net;

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
                lbl_Connection.Foreground = Brushes.LightGreen;
                isConnected = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CheckConnection();
            CheckVersions();
        }

        private void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            CheckConnection();
            CheckVersions();
        }

        private void CheckVersions()
        {
            lbl_LocalVersion.Content = "---";
            lbl_LatestVersion.Content = "---";

            int localVer=0, latestVer=0;

            /*
            if (localVer == latestVer)
                btn_Update.IsEnabled = false;
            else
                btn_Update.IsEnabled = true;
            */
        }

        private void startDownload()
        {
            lbl_Download.Content = "Download In Process...";
            btn_Update.IsEnabled = false;
            Thread thread = new Thread(() => {
                WebClient client = new WebClient();
                Uri myUrl = new Uri("http://us.cdn.persiangig.com/download/YXVPvOvzXl/BronchoVision.exe/dl");
                //client.Credentials = new NetworkCredential("Userid", "mypassword");
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                client.DownloadFileAsync(myUrl, @"BronchoVision.exe");
            });
            thread.Start();
        }
        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)delegate {
                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = bytesIn / totalBytes * 100;
                lbl_Download.Content = "Downloaded " + e.BytesReceived + " of " + e.TotalBytesToReceive;
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
