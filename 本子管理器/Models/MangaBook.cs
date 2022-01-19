using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using EroMangaManager.Database.Entities;
using EroMangaManager.Database.EntityFactory;

/*
 * 简化版EroManga类
 * 只能获取本子名，是否无修，是否全彩、是否汉化、
 * 解压第一页图至应用临时文件夹下”PrimaryPicture"文件夹
 */

using EroMangaManager.Helpers;

using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace EroMangaManager.Models
{
    /// <summary> 本子 </summary>
    public class MangaBook : INotifyPropertyChanged
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

        public ReadingInfo ReadingInfo { set; get; }

        private string _manganame;

        /// <summary> 本子名字 </summary>
        public string MangaName
        {
            get => _manganame;
            set
            {
                _manganame = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary> 实例化EroManga </summary>
        /// <param name="storageFile"> </param>
        public MangaBook (StorageFile storageFile, StorageFolder storageFolder, ReadingInfo info)
        {
            StorageFolder = storageFolder;
            StorageFile = storageFile;
            ReadingInfo = info;
            _manganame = ReadingInfo.TranslatedMangaName ?? ReadingInfo.MangaName;
        }

        /// <summary>
        /// 返回BitmapImage，作为Image控件的source
        /// </summary>
        /// <returns> </returns>
        public async Task SetCover ()
        {
            StorageFolder coverfolder = await FoldersHelper.GetCoversFolder();

            StorageFile cover = await coverfolder.GetFileAsync(StorageFile.DisplayName + ".jpg");

            BitmapImage bitmapImage = await CoverHelper.GetCoverThumbnail_SystemAsync(cover);

            Cover = bitmapImage;
        }

        public void TranslateMangaName (string name)
        {
            this.MangaName = name;
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
                    await CoverHelper.CreatThumbnailCoverFile_UsingSkiaSharp(this.StorageFile);
                }
                catch (Exception ex)
                {
                    // 上面使用的SkiaSharp库存在Bug，某些图片无法正常解码
                    IStorageItem storageItem1 = await folder.TryGetItemAsync(this.StorageFile.DisplayName + ".jpg");
                    await storageItem1?.DeleteAsync(StorageDeleteOption.PermanentDelete);

                    await CoverHelper.CreatOriginCoverFile_UsingZipArchiveEntry(this.StorageFile);
                }
            }
        }
    }
}