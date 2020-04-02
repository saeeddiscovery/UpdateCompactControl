using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Net.NetworkInformation;
using System.ComponentModel;
using System.Threading;
using System.Net;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using System.Windows.Navigation;
using Microsoft.Win32;

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
            System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            lbl_Connection.Content = "---";
            grid_Progress.Visibility = Visibility.Hidden;
            if (PingHost("8.8.8.8") == false)
            {
                lbl_Connection.Content = "Not Connected";
                lbl_Connection.Foreground = Brushes.Orange;
            }
            else
            {
                lbl_Connection.Content = "Connected";
                lbl_Connection.Foreground = Brushes.Green;
            }
            System.Windows.Input.Mouse.OverrideCursor = null;
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

        private string pathToExe = "";

        private string checkVersion_local()
        {
            string localVer = "";

            try
            {
                string filePath = Path.Combine(pathToExe, "CompactControl.exe");
                if (!File.Exists(filePath))
                {
                    var resp = MessageBox.Show("Can not find the CompactControl.exe\nPlease brows and find the file", "File Not Found", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);
                    if (resp == MessageBoxResult.OK)
                    {
                        OpenFileDialog openFileDialog = new OpenFileDialog();
                        openFileDialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
                        if (openFileDialog.ShowDialog() == true)
                        {
                            string fName = openFileDialog.FileName;
                            if (Path.GetFileName(fName) != "CompactControl.exe")
                            {
                                MessageBox.Show("The name of the file should be CompactControl.exe", "File Not Valid", MessageBoxButton.OK, MessageBoxImage.Error);
                                return "";
                            }
                            string dName = Path.GetDirectoryName(fName);
                            pathToExe = dName;
                            filePath = Path.Combine(pathToExe, "CompactControl.exe");
                        }
                    }
                }
                var versionInfo = FileVersionInfo.GetVersionInfo(filePath);
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
            System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
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

            System.Windows.Input.Mouse.OverrideCursor = null;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
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
                client.DownloadFileAsync(myUrl, Path.Combine(pathToExe, "CompactControl.zip"));


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

                string tempDir = Path.Combine(pathToExe, "tmp");
                // Extract downloaded files to the temp dir
                Directory.CreateDirectory(tempDir);
                extractToTemp(Path.Combine(pathToExe, "CompactControl.zip"), tempDir);
                string[] tmpFiles = Directory.GetFiles(tempDir);

                // Backup previous files
                string[] fileList = { "CompactControl.exe", "CompactControl.exe.config" };
                foreach (var item in fileList)
                {
                    if (File.Exists(Path.Combine(pathToExe, item + ".bak")))
                        File.Delete(Path.Combine(pathToExe, item + ".bak"));
                    if (File.Exists(Path.Combine(pathToExe, item)))
                        File.Move(Path.Combine(pathToExe, item), Path.Combine(pathToExe, item + ".bak"));
                    File.Copy(Path.Combine(tempDir, item), Path.Combine(pathToExe, item));
                }
                MessageBox.Show("Updated Successfully!", "Update Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                Directory.Delete(tempDir, true);
                File.Delete(Path.Combine(pathToExe, "CompactControl.zip"));
                CheckVersions();
            });
        }

        private void extractToTemp(string zipFile, string tempDir)
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
            ZipFile.ExtractToDirectory(zipFile, tempDir);
        }

        private void btn_Update_Click(object sender, RoutedEventArgs e)
        {
            grid_Progress.Visibility = Visibility.Visible;
            startDownload();
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            Application curApp = Application.Current;
            curApp.Shutdown();
        }
    }
}
