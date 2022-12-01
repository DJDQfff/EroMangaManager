using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using EroMangaManager.ViewModels;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace EroMangaManager.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BookcaseContainer : Page
    {
        private Dictionary<MangasFolder , Bookcase> pages = new Dictionary<MangasFolder , Bookcase>();

        public BookcaseContainer ()
        {
            this.InitializeComponent();
            MainPage.current.bookcaseContainer = this;
        }

        internal void ChangeMangasFolder(MangasFolder mangasFolder)
        {
            Bookcase bookcase;
            if(! pages.ContainsKey(mangasFolder))
            {
               bookcase=new Bookcase( mangasFolder);
                pages.Add(mangasFolder, bookcase);
            }
            else
            {
                bookcase = pages[mangasFolder];
            }
            this.Content = bookcase;

        }
    }
}
