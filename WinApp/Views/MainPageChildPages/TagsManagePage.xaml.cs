// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace WinApp.Views.MainPageChildPages;

/// <summary>......................................................................................................................................................................
/// 可用于自身或导航至 Frame 内部的空白页。
/// </summary>
public sealed partial class TagsManagePage : Page
{
    private readonly MenuFlyout menuFlyout = new();

    // 这个本来是用tagcategory.selecteditem属性的，但是不知道为什么一直为null，才额外加了一个变量

    public ManageTagsViewModel2 ViewModel { set; get; }
    MainPage MainPage { set; get; }

    /// <summary>
    ///
    /// </summary>
    public TagsManagePage(
        MainPage mainPage,
        ManageTagsViewModel2 viewmodel,
        ObservableCollectionVM observableCollectionVM
    )
    {
        InitializeComponent();
        ViewModel = viewmodel;
        MainPage = mainPage;
        var tags = observableCollectionVM.AllTags;

        ViewModel.AddUnCategoryTags(tags);
        ViewModel.CategorysChanged += MenuFlyout_SetValue;

        MenuFlyout_SetValue();
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        _ = await ViewModel.AddCategory(NewCategoryTextBox.Text);
    }

    private void Category_ListVIew_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is TagCategory a)
        {
            ViewModel.SelectedTagCategory = a;
            Tag_ListView.ItemsSource = a.Tags;
        }
    }

    private async void ImportAssemblyCategory(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem { Tag: string text } item)
        {
            var a = App
                .Services.GetRequiredService<DatabaseController>()
                .LoadCategoryFromAssembly(text);

            var b = await ViewModel.AddCategory(item.Text);
            b?.Keywords = a;
        }
    }

    private void ListViewItem_Tapped(object sender, TappedRoutedEventArgs e)
    {
        Tag_ListView.ItemsSource = ViewModel.ImCategoryedTags;
        ViewModel.SelectedTagCategory = null;
    }

    private void MenuFlyout_SetValue()
    {
        menuFlyout.Items.Clear();
        foreach (var tagcategory in ViewModel.CategoryTags)
        {
            var item = new MenuFlyoutItem
            {
                DataContext = tagcategory,
                Text = tagcategory.CategoryName,
            };
            //TODO item.IsEnabled = viewmodel.DisplayedCategory != tagcategory;
            item.Click += (s, args) =>
            {
                if (s is not MenuFlyoutItem { DataContext: TagCategory category } flyoutitem)
                {
                    return;
                }

                if (Category_ListVIew.SelectedItem is TagCategory selectedCategory)
                {
                    var tags = Tag_ListView.SelectedItems.OfType<string>().ToArray();

                    ViewModel.TagChangeCategory(selectedCategory, category, tags);
                }
            };

            menuFlyout.Items.Add(item);
        }
    }

    private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem { DataContext: string str })
        {
            var a = new SearchParameter() { Tags = [str] };
            await App
                .Services.GetRequiredService<MainPage>()
                .NavigateToPage<GlobalSearchPage>()
                .Search(a);
        }
    }

    private async void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem { DataContext: TagCategory tagcategory })
        {
            await ViewModel.DeleteCategory(tagcategory.CategoryName);
            if (tagcategory == ViewModel.SelectedTagCategory)
            {
                Tag_ListView.ItemsSource = null;
            }
        }
    }

    private async void RenameTagContentDialog(object sender, RoutedEventArgs e)
    {
        var dialog = new RenameCategoryName()
        {
            XamlRoot = App.Services.GetRequiredService<MainWindow>().Content.XamlRoot,
        };
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            if (sender is MenuFlyoutItem { DataContext: TagCategory category } item)
            {
                category.CategoryName = dialog.Newname;
                item.UpdateLayout();
            }
        }
    }
}
