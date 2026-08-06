using Server;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinApp.Views.MainPageChildPages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ServerPage : Page
{
    readonly IServiceProvider services = App.Services;
    ASPNETCoreServer? serverViewmodel;

    public ServerPage()
    {
        InitializeComponent();
    }

    private void BtnClearLog_Click(object sender, RoutedEventArgs e)
    {
        serverViewmodel?.Logs.Clear();
    }

    private async void BtnStart_Click(object sender, RoutedEventArgs e)
    {
        if (serverViewmodel is not null && serverViewmodel.IsRunning)
        {
            return;
        }

        btnStart.IsEnabled = false;
        progressRing.Visibility = Visibility.Visible;
        txtStatus.Text = "启动中...";
        statusDot.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 180, 0));

        try
        {
            serverViewmodel = new ASPNETCoreServer(
                App.Services.GetRequiredService<ObservableCollectionVM>()
            );
            logListView.ItemsSource = serverViewmodel.Logs;

            serverViewmodel.CallCoverSetterAppendWorks += async mangas =>
            {
                var tcs = new TaskCompletionSource();
                this.DispatcherQueue.TryEnqueue(() =>
                {
                    _ = App
                        .Services.GetRequiredService<CoverSetter>()
                        .MultiLoadWork(mangas, true, true)
                        .ContinueWith(_ => tcs.SetResult());
                });
                await tcs.Task;
            };

            serverViewmodel.CallCoverSetterSingleWork += async manga =>
            {
                var tcs = new TaskCompletionSource();
                this.DispatcherQueue.TryEnqueue(() =>
                {
                    _ = App
                        .Services.GetRequiredService<CoverSetter>()
                        .SingleLoadWork(manga, true, true)
                        .ContinueWith(_ => tcs.SetResult());
                });
                await tcs.Task;
            };
            serverViewmodel.AddLog += (log) =>
                this.DispatcherQueue.TryEnqueue(() => serverViewmodel.Logs.Add(log));

            serverViewmodel.EventDeleteMang += async manga =>
                _ = await services.GetRequiredService<StorageOperation>().Delete(manga);

            serverViewmodel.EventDeleteMang += async manga =>
            {
                var result = this.DispatcherQueue.TryEnqueue(() =>
                    App.Services.GetRequiredService<ObservableCollectionVM>().RemoveManga(manga)
                );

                return result;
            };
            await serverViewmodel.StartServer();

            this.DataContext = serverViewmodel;

            serverViewmodel.ValidatePort(12965);
            // TODO 如果设备安装了虚拟网卡（如hyperv等虚拟机），获取的ip不是正确ip
            txtFullAddress.Text = serverViewmodel.FullAddress;
            txtStatus.Text = "运行中";
            statusDot.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 200, 0));
            btnStart.Content = "服务已启动";
        }
        catch (Exception ex)
        {
            txtStatus.Text = "启动失败";
            statusDot.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 68, 68));
            btnStart.IsEnabled = true;

            _ = new ContentDialog
            {
                Title = "启动失败",
                Content = ex.Message,
                CloseButtonText = "确定",
            }.ShowAsync();
        }
        finally
        {
            progressRing.Visibility = Visibility.Collapsed;
        }
    }
}
