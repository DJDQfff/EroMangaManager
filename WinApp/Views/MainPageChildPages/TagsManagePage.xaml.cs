// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板
namespace EroMangaManager.WinUI3.Views.MainPageChildPages
{
    /// <summary>......................................................................................................................................................................
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class TagsManagePage : Page
    {
        private readonly MenuFlyout menuFlyout = new();

        // 这个本来是用tagcategory.selecteditem属性的，但是不知道为什么一直为null，才额外加了一个变量

        private readonly ManageTagsViewModel2 viewmodel = new();

        /// <summary>
        ///
        /// </summary>
        public TagsManagePage()
        {
            InitializeComponent();
            var tags = App.Current.GlobalViewModel.AllTags;

            viewmodel.AddUnCategoryTags(tags);
            viewmodel.CategorysChanged += MenuFlyout_SetValue;

            MenuFlyout_SetValue();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _ = viewmodel.AddCategory(NewCategoryTextBox.Text);
        }

        private void Category_ListVIew_ItemClick(object sender, ItemClickEventArgs e)
        {
            var a = e.ClickedItem as TagCategory;
            viewmodel.selectedTagCategory = a;
            Tag_ListView.ItemsSource = a.Tags;
        }

        private void ImportAssemblyCategory(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuFlyoutItem;
            var text = item.Tag as string;// 这里不能直接用text，因为下面的方法需要区分中英文
            var a = DatabaseController.LoadCategoryFromAssembly(text);

            var b = viewmodel.AddCategory(item.Text);
            if (b is not null)
            {
                b.Keywords = a;
            }
        }

        private void ListViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Tag_ListView.ItemsSource = viewmodel.ImCategoryedTags;
            viewmodel.selectedTagCategory = null;
        }

        private void MenuFlyout_SetValue()
        {
            menuFlyout.Items.Clear();
            foreach (var tagcategory in viewmodel.CategoryTags)
            {
                var item = new MenuFlyoutItem
                {
                    DataContext = tagcategory,
                    Text = tagcategory.CategoryName
                };
                //TODO item.IsEnabled = viewmodel.DisplayedCategory != tagcategory;
                item.Click += (s, args) =>
                {
                    var flyoutitem = s as MenuFlyoutItem;
                    var category = flyoutitem.DataContext as TagCategory;
                    var tags = Tag_ListView.SelectedItems.Select(x => x as string).ToArray();

                    var selectedCategory = Category_ListVIew.SelectedItem as TagCategory;

                    viewmodel.TagChangeCategory(selectedCategory, category, tags);
                };

                menuFlyout.Items.Add(item);
            }
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuFlyoutItem;

            if (item.DataContext is string str)
            {
                var a = new SearchParameter() { Tags = [str] };
                MainPage.Current.MainFrame.Navigate(typeof(GlobalSearchPage), a);
            }
        }

        private void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuFlyoutItem;
            var tagcategory = item.DataContext as TagCategory;

            viewmodel.DeleteCategory(tagcategory.CategoryName);
            if (tagcategory == viewmodel.selectedTagCategory)
            {
                Tag_ListView.ItemsSource = null;
            }
        }

        private async void RenameTagContentDialog(object sender, RoutedEventArgs e)
        {
            var dialog = new RenameCategoryName()
            {
                XamlRoot = App.Current.MainWindow.Content.XamlRoot
            };
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var item = sender as MenuFlyoutItem;
                var category = item.DataContext as TagCategory;
                category.CategoryName = dialog.Newname;
                item.UpdateLayout();
            }
        }
    }
}