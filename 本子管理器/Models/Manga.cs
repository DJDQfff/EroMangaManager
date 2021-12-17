using System;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel;
using Abandoned.EroMangaManager.Models;

/*
 * 简化版EroManga类
 * 只能获取本子名，是否无修，是否全彩、是否汉化、
 * 解压第一页图至应用临时文件夹下”PrimaryPicture"文件夹
 */

using EroMangaManager.Helpers;

using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;
using System.Runtime.CompilerServices;

namespace EroMangaManager.Models
{
    /// <summary> 本子 </summary>
    public class Manga : INotifyPropertyChanged
    {
        /// <summary> 封面缩略图 </summary>
        //public BitmapImage CoverImage { private set; get; } = new BitmapImage();

        public event PropertyChangedEventHandler PropertyChanged;

        private ImageSource imagesource = new SvgImageSource(new Uri("ms-appx:///Assets/SVGs/书籍.svg"));

        public ImageSource Cover
        {
            set
            {
                imagesource = value;
                NotifyPropertyChanged();
            }
            get => imagesource;
        }

        /// <summary> 本子对应具体文件 </summary>
        public StorageFile StorageFile { get; }

        /// <summary> 本子所属文件夹 </summary>
        public StorageFolder StorageFolder { set; get; }

        /// <summary> 本子文件全名（不带扩展名） </summary>
        private readonly string fileDisplayname;

        private string mangaName;

        /// <summary> 本子名字 </summary>
        public string MangaName
        {
            get => mangaName;
            set
            {
                mangaName = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary> 是否无修，ture为无修，默认为false </summary>
        public bool Tag_NonMosaic { private set; get; } = false;

        /// <summary> 是否全彩，true是全彩，默认为false </summary>
        public bool Tag_FullColor { private set; get; } = false;

        /// <summary> 语言,日，中，英 </summary>
        public string Tag_Language { get; } = "jp";

        /// <summary> 实例化EroManga </summary>
        /// <param name="storageFile"> </param>
        public Manga (StorageFile storageFile, StorageFolder storageFolder)
        {
            StorageFolder = storageFolder;
            StorageFile = storageFile;
            fileDisplayname = storageFile.DisplayName;
            SetTag_NonMosaicAndColor();
            InitializeMangaName(fileDisplayname);
        }

        /// <summary>
        /// 返回BitmapImage，作为Image控件的source
        /// </summary>
        /// <returns> </returns>
        public async Task SetCover ()
        {
            StorageFolder coverfolder = await FoldersHelper.GetCoversFolder();

            StorageFile cover = await coverfolder.GetFileAsync(StorageFile.DisplayName + ".jpg");

            BitmapImage bitmapImage = await CoverHelper.GetCoverThumbnail_CustomAsync(cover);

            Cover = bitmapImage;
        }

        public void SetTranslateName (string name)
        {
            this.MangaName = name;
        }

        /// <summary> 设置本子的名字 </summary>
        /// <param name="fullname"> </param>
        private void InitializeMangaName (string fullname)
        {
            fullname = fullname.Trim();
            int index = 0;
            switch (fullname[0])
            {
                case '[':
                    {
                        index = fullname.IndexOf(']');
                        goto Include;
                    }
                case '【':
                    {
                        index = fullname.IndexOf('】');
                        goto Include;
                    }
                case '（':
                    {
                        index = fullname.IndexOf('）');
                        goto Include;
                    }
                case '(':
                    {
                        index = fullname.IndexOf(')');
                        goto Include;
                    }
                default:
                    goto NotInclude;
            }

        NotInclude:
            index = fullname.IndexOfAny(new char[] { '[', '【', '（', '(' });
            if (index != -1)
            {
                mangaName = fullname.Substring(0, index).Trim();
                return;
            }
            else
            { mangaName = fullname; return; }//剩到最后的部分
        Include:
            if (index != -1)
            {
                fullname = fullname.Substring(index + 1);
                InitializeMangaName(fullname);
            }
            else
            { mangaName = "请补全括号"; return; }    // TODO：这里要改
        }

        /// <summary> 判断本子是否无修及是否全彩 </summary>
        private void SetTag_NonMosaicAndColor ()
        {
            if (fileDisplayname.Contains("无修") || fileDisplayname.Contains("無修"))
            {
                Tag_NonMosaic = true;
            }

            if (fileDisplayname.Contains("全彩"))
            {
                Tag_FullColor = true;
            }
        }

        /// <summary> 删除文件 </summary>
        /// <returns> </returns>
        public async Task RemoveFile ()
        {
            await this.StorageFile.DeleteAsync();
        }

        private void NotifyPropertyChanged ([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary> 确保生成封面 </summary>
        /// <returns> </returns>
        public async Task EnsureCoverFile ()
        {
            StorageFolder folder = await FoldersHelper.GetCoversFolder();
            IStorageItem storageItem = await folder.TryGetItemAsync(this.StorageFile.DisplayName + ".jpg");
            if (storageItem is null)
            {
                try
                {
                    await CoverHelper.CreatCoverFile(this.StorageFile);
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}