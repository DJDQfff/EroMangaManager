using Windows.Foundation;

namespace UnoApp;

public sealed partial class ConnectDialog : ContentDialog
{
    private readonly ServerStorage _serverStorage;
    private readonly MangaAPIClient _mangaApiClient;

    public ConnectDialog (ServerStorage serverStorage , MangaAPIClient mangaApiClient)
    {
        InitializeComponent();
        _serverStorage = serverStorage;
        _mangaApiClient = mangaApiClient;

        // 加载上次保存的服务器
        var lastServer = _serverStorage.LoadLastServer();
        if (!string.IsNullOrEmpty(lastServer))
        {
            ParseUrlToUI(lastServer);
        }
    }

    // ... (GetServerUrl 和 ParseUrlToUI 保持不变) ...
    private string GetServerUrl ()
    {
        int ip3 = (int) Math.Round(nbIP3.Value);
        int ip4 = (int) Math.Round(nbIP4.Value);
        if (ip3 < 0 || ip3 > 255 || ip4 < 0 || ip4 > 255)
            return string.Empty;
        return $"http://192.168.{ip3}.{ip4}:{(int) Math.Round(nbPort.Value)}";
    }

    private void ParseUrlToUI (string url)
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

    // 3. 核心拦截逻辑
    private async void ConnectDialog_PrimaryButtonClick (ContentDialog sender , ContentDialogButtonClickEventArgs args)
    {
        // 1. 【关键】：申请延期，阻止对话框在 await 之后自动关闭
        var deferral = args.GetDeferral();

        string url = GetServerUrl();
        if (string.IsNullOrEmpty(url))
        {
            ShowError("请输入有效的 IP 地址和端口");
            args.Cancel = true;
            deferral.Complete(); // 2. 提前退出时，必须手动完成延期
            return;
        }

        SetLoading(true);
        txtError.Visibility = Visibility.Collapsed;

        try
        {
            _mangaApiClient.UpdateHttpClient(url);
            // 执行连接检查
           await  _mangaApiClient.CheckConnectionAsync();

            // 成功则保存并更新
            _serverStorage.SaveServer(url);

            // 不调用 args.Cancel，允许对话框关闭
        }
        catch (HttpRequestException)
        {
            ShowError("无法连接到服务器，请检查 IP 和端口");
            args.Cancel = true; // 失败，拦截关闭
        }
        catch (TaskCanceledException)
        {
            ShowError("连接超时，请确认在同一网络");
            args.Cancel = true;
        }
        catch (Exception ex)
        {
            ShowError($"连接失败: {ex.Message}");
            args.Cancel = true;
        }
        finally
        {
            SetLoading(false);
            // 4. 【关键】：无论成功还是失败，都必须调用 Complete() 释放延期
            deferral.Complete();
        }
    }

    private void ShowError (string message)
    {
        txtError.Text = message;
        txtError.Visibility = Visibility.Visible;
    }

    private void SetLoading (bool isLoading)
    {
        progressBar.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
        IsPrimaryButtonEnabled = !isLoading; // 加载时禁用按钮防抖
    }

}