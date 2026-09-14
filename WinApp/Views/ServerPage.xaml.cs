using Server;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinApp.Views.MainPageChildPages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ServerPage : Page
{
    ASPNETCoreServer ServerViewmodel { get; } = null!;

    public ServerPage(ASPNETCoreServer _serverViewmodel)
    {
        InitializeComponent();
        this.ServerViewmodel = _serverViewmodel;
    }

    private async void BtnStart_Click(object sender, RoutedEventArgs e)
    {
        if (!ServerViewmodel.IsRunning)
        {
            btnStart.IsEnabled = false;
            progressRing.Visibility = Visibility.Visible;
            txtStatus.Text = "启动中...";
            statusDot.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 180, 0));

            try
            {
                logListView.ItemsSource = ServerViewmodel.Logs;

                ServerViewmodel.CallCoverSetterAppendWorks += async mangas =>
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

                ServerViewmodel.CallCoverSetterSingleWork += async manga =>
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
                ServerViewmodel.AddLog += (log) =>
                    this.DispatcherQueue.TryEnqueue(() => ServerViewmodel.Logs.Add(log));

                ServerViewmodel.EventDeleteMang += async manga =>
                {
                    try
                    {
                        await ServerViewmodel.DeleteManga(manga);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                };

                ServerViewmodel.EventDeleteMang += async manga =>
                {
                    var result = this.DispatcherQueue.TryEnqueue(() =>
                        ServerViewmodel.RemoveManga(manga)
                    );

                    return result;
                };
                await ServerViewmodel.StartServer();

                this.DataContext = ServerViewmodel;

                ServerViewmodel.ValidatePort(12965);
                // TODO 如果设备安装了虚拟网卡（如hyperv等虚拟机），获取的ip不是正确ip
                txtFullAddress.Text = ServerViewmodel.FullAddress;
                txtStatus.Text = "运行中";
                statusDot.Background = new SolidColorBrush(
                    Windows.UI.Color.FromArgb(255, 0, 200, 0)
                );
                btnStart.Content = "服务已启动";
            }
            catch (Exception ex)
            {
                txtStatus.Text = "启动失败";
                statusDot.Background = new SolidColorBrush(
                    Windows.UI.Color.FromArgb(255, 255, 68, 68)
                );
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
}
