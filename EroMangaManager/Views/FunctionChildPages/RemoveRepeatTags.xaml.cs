using System.Collections.Generic;
using System.Linq;
using MyStandard20Library;
using EroMangaManager.Models;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace EroMangaManager.Views.FunctionChildPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class RemoveRepeatTags : Page
    {
        /// <summary>
        ///
        /// </summary>
        public RemoveRepeatTags ()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo (NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var containrepeat = new List<MangaBook>();
            foreach(var book in App.Current.collectionObserver.MangaList)
            {
                if (book.MangaTagsIncludedInFileName.ContainRepeat())
                {
                    containrepeat.Add(book);
                }
            }
            list.ItemsSource = containrepeat;
        }
    }
}