// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace WinApp.Views.FunctionChildPages;

/// <summary>
/// 可用于自身或导航至 Frame 内部的空白页。
/// </summary>
public sealed partial class FindSameManga : Page
{
    private readonly SameMangaSearchViewModel viewModel = new();
    private readonly ObservableCollectionVM observableCollectionVM;
    private CancellationTokenSource cancellationTokenSource = new();

    /// <summary>
    ///
    /// </summary>
    public FindSameManga(ObservableCollectionVM _observableCollectionVM, CoverSetter _coverSetter)
    {
        InitializeComponent();
        this.observableCollectionVM = _observableCollectionVM;
        _observableCollectionVM.EventAfterDeleteMangaSource +=
            viewModel.DeleteStorageFileInRootObservable;
        //viewModel.AddToResult += x => App.Current.BackgroundCoverSetter.mangas.Insert(0, x);
        viewModel.AddGroup += async x =>
            await _coverSetter.MultiLoadWork(x.Collections, true, true);
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        if (cancellationTokenSource is null)
            return;

        cancellationTokenSource.Cancel();

        cancellationTokenSource = new();
        viewModel.Source =
        [
            .. observableCollectionVM.MangaList.SkipWhile(x => string.IsNullOrWhiteSpace(x.Name)),
        ];

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
                    // 可能有bug，没有设置xaml content
                    var selectcategory = App.Services.GetRequiredService<TagCategorySelect>();
                    var result = await selectcategory.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        if (!string.IsNullOrWhiteSpace(selectcategory.CategoryName))
                        {
                            var strings = App
                                .Services.GetRequiredService<DatabaseController>()
                                .TagCategory_QuerySingle(selectcategory.CategoryName);
                            await viewModel.Method3_1(strings, cancellationTokenSource);
                        }
                    }
                }
                break;
        }
    }
}
