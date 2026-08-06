using Core.Models;

namespace UnoApp;

public sealed partial class NavigationPage : Page
{
    public RemoteMangaViewModel ViewModel { get; } = new(App.MangaClient);

    public NavigationPage()
    {
        this.InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.GetGroups();

        await ViewModel.SelectFirst();
    }

    private async void Gridview_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is Manga manga)
        {
            var container = MangaListView.ContainerFromItem(manga) as ListViewItem;
            //var grid = container?.FindFirstDescendant<Grid>();
            var progressbar = container?.FindName("progressbar") as ProgressBar;
            progressbar?.Visibility = Visibility.Visible;
            await ViewModel.OpenAsync(manga);
            progressbar!.Visibility = Visibility.Collapsed;
        }

        //TODO 下面可以显示下载进度，但安卓端有问题，暂时先直接下载后打开
        // 1.下载速度跑不满
        //2. progress.Report()调用会影响下载速度。，即便不使用progress.Report()，下载速度也只有2M多，怀疑是因为每次读写都要调用UI线程的关系。
        // 3.频繁调用性能网速更低，怀疑是因为每次读写都要调用UI线程的关系。

        // 此方法在安卓端错误
        //var root = container.ContentTemplateRootas StackPanel;
        //var progressbar = root?.FindName("progressbar") as ProgressBar;

        //var progressbar = container?.FindFirstDescendant<ProgressBar>();

        //IProgress<float> progress = new Progress<float>(value =>
        //{
        //    // 这里的代码会自动运行在 UI 线程上
        //    progressbar.Value = value;

        //});
        //// 1. 先获取响应头，不要直接读内容流
        //var response = await ViewModel.GetAsync($"/downloads/{manga.Guid}", HttpCompletionOption.ResponseHeadersRead);
        //response.EnsureSuccessStatusCode();

        //// 2. 获取总/近似大小(文件夹则按近似大小，单文件则按实际大小)，以之为进度条最大值。
        //long estimatedSize = 0;
        //if (response.Content.Headers.ContentLength.HasValue)
        //{
        //    estimatedSize = response.Content.Headers.ContentLength.Value;
        //}
        //else if (response.Headers.TryGetValues("X-Estimated-Size", out var values))
        //{
        //    estimatedSize = long.Parse(values.FirstOrDefault());
        //}
        //// 若大小为0，则不启用进度条
        //if (estimatedSize != 0)
        //{
        //    progressbar.Visibility = Visibility.Visible;
        //    progressbar.Maximum = (double)estimatedSize;

        //}

        //using var networkstream = await response.Content.ReadAsStreamAsync();
        //using (var fileStream = await storagefile.OpenStreamForWriteAsync())
        //{
        //    var buffer = new byte[1048576]; // 64KB 缓冲区，提高下载速度。可选 1048576  65536
        //    long bytesRead = 0;
        //    int read;

        //    while ((read = await networkstream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        //    {
        //        await fileStream.WriteAsync(buffer, 0, read);

        //        bytesRead += read;

        //        // 3. 报告进度 (如果是 null 则跳过)
        //        //if (estimatedSize > 0 && progress != null)
        //        //{
        //        //    // 计算百分比，这是默认progressbar是最大100的时候，不用了
        //        //    //float percent =  (float)bytesRead / totalBytes.Value*100;

        //        //    progress.Report(bytesRead);
        //        //}
        //    }
        //    //progress.Report(estimatedSize); // 确保最后报告完成

        //}

        //AndroidOperation.Open(storagefile);
    }

    //private void Image_Loaded(object sender, RoutedEventArgs e)
    //{
    //    var image = sender as Image;
    //    var manga = image.DataContext as MangaDTO;
    //    var uri = new System.Uri($"{ViewModel.BaseAddress}covers/{manga.Guid}");
    //    image.Source = new BitmapImage(uri);
    //}

    private async void Navigationview_ItemInvoked(
        NavigationView sender,
        NavigationViewItemInvokedEventArgs args
    )
    {
        switch (args.InvokedItemContainer.Content)
        {
            case "随机本子":
                {
                    await ViewModel.UpdateSelectedGroupRandomManga();
                }
                break;
            case "随机标签": { }

                break;
        }
    }

    private async void Numberbox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (navigationview.SelectedItem is MangasGroupDTO selectedGroup)
        {
            var count = ViewModel.Mangas.Count;

            var newvalue = (int)args.NewValue;

            var oldvalue = (int)args.OldValue;

            if (newvalue > oldvalue)
            {
                await ViewModel.UpdateCollection(
                    selectedGroup,
                    (newvalue - 2) * ViewModel.DisplayAmount + count
                );
            }
            else
            {
                await ViewModel.UpdateCollection(
                    selectedGroup,
                    (newvalue - 1) * ViewModel.DisplayAmount + count
                );
            }
        }
    }

    private void MenuFlyout_Opened(object sender, object e)
    {
        if (sender is MenuFlyout { Target.DataContext: Manga currentManga } menuFlyout)
        {
            if (menuFlyout.Items.First() is not MenuFlyoutItem deleteitem)
            {
                return;
            }
            // 无法在menuflyout里绑定到项，flyout写在resource里面后，绑定的源不是激活这个flyout的项
            // 使用CommandParameter={Binding}获取到的是集合的最后一项。
            // ai解释说：因为作为公用对象，所以只在创建的时候绑定了一次，即便后面触发源变了但绑定源没更新
            deleteitem.CommandParameter = currentManga;

            // 这个也是一样，只能获取到集合最后一项，即便代码里设置了子item，显示的照样还是旧内容
            // 直接移除旧的，
            // 🔥 找到并移除旧的 SubItem
            var oldSubItem = menuFlyout.Items.OfType<MenuFlyoutSubItem>().FirstOrDefault();
            if (oldSubItem != null)
            {
                menuFlyout.Items.Remove(oldSubItem);
            }

            // 🔥 创建全新的 SubItem
            var tagSubItem = new MenuFlyoutSubItem { Text = "搜索Tag" };
            foreach (var tag in currentManga.Tags)
            {
                MenuFlyoutItem menuItem = new()
                {
                    Text = tag,
                    Tag = tag, // 把 tag 字符串存在 Tag 属性里
                    Command = ViewModel.SearchByTagCommand,
                    CommandParameter = tag,
                };
                // 添加到子菜单中
                tagSubItem.Items.Add(menuItem);
            }
            // 添加到菜单
            menuFlyout.Items.Add(tagSubItem);
            tagSubItem.UpdateLayout();
        }
    }
}
