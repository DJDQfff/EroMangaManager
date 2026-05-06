// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

using System.Threading;

namespace EroMangaManager.WinUI3.Views.FunctionChildPages;

/// <summary>
/// 可用于自身或导航至 Frame 内部的空白页。
/// </summary>
public sealed partial class FindSameManga : Page
{
    private readonly SameMangaSearchViewModel viewModel = new();

    private CancellationTokenSource cancellationTokenSource = new();

    /// <summary>
    ///
    /// </summary>
    public FindSameManga()
    {
        InitializeComponent();
        App.Current.GlobalViewModel.EventAfterDeleteMangaSource += viewModel.DeleteStorageFileInRootObservable;
        //viewModel.AddToResult += x => App.Current.BackgroundCoverSetter.mangas.Insert(0, x);
        viewModel.AddGroup += async x => await App.Current.CoverSetter.AddWork(x.Collections);
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        if (cancellationTokenSource is null)
            return;

        cancellationTokenSource.Cancel();

        cancellationTokenSource = new();
        viewModel.Source = App.Current.GlobalViewModel.MangaList.SkipWhile(x => string.IsNullOrWhiteSpace(x.MangaName)).ToList();

        switch (combobox.SelectedIndex)
        {
            case 0:
                await viewModel.Method0(cancellationTokenSource);
                break;

            case 1:
                await viewModel.Method1();
                break;

            case 2:
                await viewModel.Method2(cancellationTokenSource);
                break;

            case 3:
                {
                    var selectcategory = new TagCategorySelect()
                    {
                        XamlRoot = App.Current.MainWindow.Content.XamlRoot
                    };
                    var result = await selectcategory.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        if (!string.IsNullOrWhiteSpace(selectcategory.CategoryName))
                        {
                            var strings = DatabaseController.TagCategory_QuerySingle(selectcategory.CategoryName);
                            await viewModel.Method3_1(strings, cancellationTokenSource);
                        }
                    }
                }
                break;
        }
    }
}