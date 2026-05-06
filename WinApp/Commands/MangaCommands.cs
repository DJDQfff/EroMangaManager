using Microsoft.Windows.AppNotifications.Builder;
namespace EroMangaManager.WinUI3.Commands;

internal class MangaCommands
{
    public StandardUICommand ExportPDF = new(StandardUICommandKind.Save);
    public StandardUICommand LocateInExplorer = new(StandardUICommandKind.Open);
    //public StandardUICommand OpenManga = new(StandardUICommandKind.Open);
    public StandardUICommand OpenWithManga = new(StandardUICommandKind.Open);
    public StandardUICommand StorageCommandDelete = new(StandardUICommandKind.Delete);
    public StandardUICommand StorageCommandRename = new();
    public static MangaCommands Instance { get; private set; }

    public static void Initial()
    {
        Instance ??= new();

        Instance.LocateInExplorer.ExecuteRequested += (sender, args) =>
        {
            string folderpath = args.Parameter switch
            {
                MangasGroup folder => folder.FolderPath,
                Manga book => book.FilePath,
                string str => str,
                _ => null
            };
            ;
            if (folderpath is not null)
            {
                ExplorerFile.ExplorerSelectFile(folderpath);
            }
            //System.Diagnostics.Process.Start("explorer" , $"/select , {folderpath}");
        };
        Instance.StorageCommandDelete.IconSource = new SymbolIconSource()
        {
            Symbol = Symbol.Delete
        };
        Instance.StorageCommandDelete.ExecuteRequested += async (sender, args) =>
        {
            switch (args.Parameter)
            {
                case Manga book:
                    {
                        try
                        {
                            var result = await DialogHelper.ConfirmDeleteSourceFileDialog(book);
                            if (result)
                            {
                                App.Current.GlobalViewModel.RemoveManga(book);
                                App.Current.GlobalViewModel.InvokeEvent_AfterDeleteMnagaSource(book);
                            }
                        }
                        catch (UnauthorizedAccessException)
                        {
                            App.Current.GlobalViewModel.AccessDenied();
                        }
                        catch (System.IO.IOException)
                        {
                            App.Current.GlobalViewModel.AccessDenied();
                        }

                        //_ = await App.Current.GlobalViewModel.TryRemoveManga(book);
                    }
                    break;
            }
        };

        Instance.StorageCommandRename.ExecuteRequested += async (sender, args) =>
        {
            switch (args.Parameter)
            {
                case Manga book:
                    await DialogHelper.RenameSourceFileInDialog(book);
                    break;
            }
        };
        Instance.StorageCommandRename.IconSource = new SymbolIconSource()
        {
            Symbol = Symbol.Rename
        };

        Instance.OpenWithManga.ExecuteRequested += async (sender, args) =>
        {
            Manga manga = default;
            string way = "explorer.exe";// 默认值
            switch (args.Parameter)
            {
                case Manga manga1:
                    {
                        manga = manga1;
                        way = App.Current.AppConfig.AppConfig.MangaOpenWay3.DefaultWay;
                    }
                    break;
                case (Manga manga1, string way1):
                    {
                        manga = manga1;
                        way = way1;
                    }
                    break;

            }

            try
            {
                await Process.Start(way, $"\"{manga.FilePath}\"").WaitForExitAsync();

                return;
                // 有bug，线程会冲突
                if (MangaFileIO.Exists(manga))
                {
                    await MangaFileIO.LoadMangaInfo(manga);
                    manga.CoverPath = await Task.Run(async () => await MangaFactory.GetCoverFile(manga));
                }
                else
                {
                    App.Current.GlobalViewModel.RemoveManga(manga);
                    App.Current.GlobalViewModel.InvokeEvent_AfterDeleteMnagaSource(manga);
                }



            }
            catch (Exception)
            {
                var appNotification = new AppNotificationBuilder()
                    .AddText(
                        $"{manga.MangaName}\r{ResourceLoader.GetForViewIndependentUse().GetString("OpenFailed")}"
                    )
                    .BuildNotification();
                AppNotificationManager.Default.Show(appNotification);
            }

        };

        // 弃用
        //Instance.OpenManga.ExecuteRequested += async (sender, args) =>
        //{


        //    if (args.Parameter is Manga manga)
        //    {
        //        var wayindex = App.Current.AppConfig.AppConfig.MangaOpenWay3.WayIndex;

        //        try
        //        {
        //            switch (wayindex)
        //            {
        //                // 功能不完善，注销掉
        //                //case 0:
        //                //RunDefault:
        //                //    WindowHelper.ShowReadWindow(manga);
        //                //    break;

        //                case 1:

        //                    Process.Start("explorer", manga.FilePath);
        //                    break;

        //                case > 1:
        //                    var SelectedExePath = App.Current.AppConfig.ExePaths.ToList()[wayindex - 2];
        //                    if (!File.Exists(SelectedExePath))
        //                    {
        //                        //goto RunDefault;
        //                    }
        //                    else
        //                    {
        //                        await Process.Start(SelectedExePath, $"\"{manga.FilePath}\"").WaitForExitAsync();
        //                        if (MangaFactory.Exists(manga))
        //                        {
        //                            await MangaFactory.LoadMangaInfo(manga);
        //                        }
        //                        else
        //                        {
        //                            App.Current.GlobalViewModel.RemoveManga(manga);
        //                            App.Current.GlobalViewModel.InvokeEvent_AfterDeleteMnagaSource(manga);
        //                        }
        //                    }
        //                    break;
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            var appNotification = new AppNotificationBuilder()
        //                .AddText(
        //                    $"{manga.MangaName}\r{ResourceLoader.GetForViewIndependentUse().GetString("OpenFailed")}"
        //                )
        //                .BuildNotification();
        //            AppNotificationManager.Default.Show(appNotification);
        //        }
        //    }
        //};

        Instance.ExportPDF.ExecuteRequested += async (sender, args) =>
        {
            switch (args.Parameter)
            {
                case Manga book:
                    await StorageOperation.ExportAsPDFAsync(book);
                    break;
            }
        };
        Instance.ExportPDF.IconSource = new SymbolIconSource() { Symbol = Symbol.Save, };
    }
}