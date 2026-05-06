using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace EroMangaManagerHttpApiClient
{
    public sealed partial class ConnectPage : Page
    {
        public ConnectPage()
        {
            this.InitializeComponent();

            string lastServer = ServerStorage.LoadLastServer();
            if (!string.IsNullOrEmpty(lastServer))
            {
                ParseUrlToUI(lastServer);
            }
        }

        private string GetServerUrl()
        {
            int ip3 = (int)Math.Round(nbIP3.Value);
            int ip4 = (int)Math.Round(nbIP4.Value);

            if (ip3 < 0 || ip3 > 255 || ip4 < 0 || ip4 > 255)
                return string.Empty;

            string ip = $"192.168.{ip3}.{ip4}";
            string port = ((int)Math.Round(nbPort.Value)).ToString();
            return $"http://{ip}:{port}";
        }
        private void ParseUrlToUI(string url)
        {
            try
            {
                var uri = new Uri(url);
                string[] ipParts = uri.Host.Split('.');
                if (ipParts.Length == 4)
                {
                    nbIP3.Value = double.Parse(ipParts[2]);
                    nbIP4.Value = double.Parse(ipParts[3]);
                }
                nbPort.Value = uri.Port;
            }
            catch { }
        }

        private async void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            string url = GetServerUrl();

            SetLoading(true);
            txtError.Visibility = Visibility.Collapsed;

            try
            {
                var client = new MangaAPIClient(url);
                bool isConnected = await client.CheckConnectionAsync();

                if (!isConnected)
                {
                    ShowError("无法连接到服务器");
                    return;
                }

                ServerStorage.SaveServer(url);
                App.MangaClient = client;
                Frame.Navigate(typeof(NavigationPage));
            }
            catch (HttpRequestException)
            {
                ShowError("无法连接到服务器，请检查 IP 地址和端口是否正确");
            }
            catch (TaskCanceledException)
            {
                ShowError("连接超时，请确认手机和电脑在同一网络");
            }
            catch (Exception ex)
            {
                ShowError($"连接失败: {ex.Message}");
            }
            finally
            {
                SetLoading(false);
            }
        }

        private void ShowError(string message)
        {
            txtError.Text = message;
            txtError.Visibility = Visibility.Visible;
        }

        private void SetLoading(bool isLoading)
        {
            progressRing.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
            btnConnect.IsEnabled = !isLoading;
        }
    }
}
