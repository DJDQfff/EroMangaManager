using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using EroMangaManager.Models;
using EroMangaTagDatabase.Entities;
using EroMangaTagDatabase.EntityFactory;
using EroMangaTagDatabase.DatabaseOperation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace EroMangaManager.InteractPage
{
    public sealed partial class MangaTagEdit : ContentDialog
    {
        private MangaBook mangaBook;
        private ObservableCollection<string> taglist;

        public MangaTagEdit (MangaBook _mangaBook)
        {
            this.InitializeComponent();
            mangaBook = _mangaBook;
            var tags = _mangaBook.ReadingInfo.TagPieces.Split('\r');
            taglist = new ObservableCollection<string>(tags);
            taglist.Add(_mangaBook.ReadingInfo.MangaName);
        }

        private async Task ContentDialog_PrimaryButtonClick (ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            foreach (var item in taglist)
            {
                var listviewitem = List.ContainerFromItem(item) as ListViewItem;
                var relativepanel = listviewitem.ContentTemplateRoot as RelativePanel;
                var textblock = relativepanel.FindName("str") as TextBlock;
                var tag = textblock.Text;
                var combobox = relativepanel.FindName("combo") as ComboBox;
                var index = combobox.SelectedIndex;

                switch (index)
                {
                    case 0:
                        await ReadingInfoTableOperation.UpdateMangaName(mangaBook.ReadingInfo, tag);
                        break;

                    case 1:
                        await TagKeywordsOperation.UpdateAppendSingle(TagType.authorTags.ToString(), tag);
                        break;

                    case 2:
                        await TagKeywordsOperation.UpdateAppendSingle(TagType.tra.ToString(), tag);
                        break;

                    case 3:
                        await TagKeywordsOperation.UpdateAppendSingle(TagType.authorTags.ToString(), tag);
                        break;

                    case 4:
                        await TagKeywordsOperation.UpdateAppendSingle(TagType.authorTags.ToString(), tag);
                        break;

                    case 5:
                        await TagKeywordsOperation.UpdateAppendSingle(TagType.authorTags.ToString(), tag);
                        break;

                    case 6:
                        await TagKeywordsOperation.UpdateAppendSingle(TagType.authorTags.ToString(), tag);
                        break;

                    case 7:
                        await TagKeywordsOperation.UpdateAppendSingle(TagType.authorTags.ToString(), tag);
                        break;
                }
            }
        }
    }
}