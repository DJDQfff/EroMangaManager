using System;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using EroMangaTagDatabase.Entities;
using EroMangaTagDatabase.EntityFactory;
using static MyUWPLibrary.StorageFolderHelper;
using static EroMangaManager.Models.FolderEnum;

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

        /// <summary> 文件路径 </summary>
        private string _filePath;

        /// <summary> 获取文件的扩展名 </summary>
        public string ExtensionName => Path.GetExtension(_filePath).ToLower();

        /// <summary> 文件Display名（不带扩展名） </summary>
        public string FileDisplayName => Path.GetFileNameWithoutExtension(_filePath);

        /// <summary> 漫画文件路径 </summary>
        public string FilePath => _filePath;

        /// <summary> 漫画所在文件夹路径 </summary>
        public string FolderPath => Path.GetDirectoryName(_filePath);

        /// <summary> 漫画文件名（全名，带扩展名） </summary>
        public string FileName => Path.GetFileName(_filePath);

        /// <summary> 本子对应具体文件 </summary>
        public StorageFile StorageFile { get; }

        /// <summary> 本子所属文件夹 </summary>
        public StorageFolder StorageFolder { set; get; }

        public ReadingInfo ReadingInfo { set; get; }

        /// <summary> 本子名字 </summary>
        /// <summary> 漫画名称 </summary>
        public virtual string MangaName
        {
            get => ReadingInfo.MangaName;
            set
            {
                ReadingInfo.MangaName = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary> 本子Tag </summary>
        public string[] MangaTags => ReadingInfo.TagPieces.Split('\r');

        /// <summary> 漫画翻译后的断名称 </summary>
        public string TranslatedMangaName
        {
            get => ReadingInfo.MangaName_Translated;
            set
            {
                ReadingInfo.MangaName_Translated = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary> 实例化EroManga </summary>
        /// <param name="storageFile"> </param>
        public MangaBook (StorageFile storageFile, StorageFolder storageFolder, ReadingInfo info)
        {
            string path = storageFile.Path;
            if (Path.GetExtension(path).ToLower() != ".zip")
            {
                //TODO 这个可以删掉
                throw new Exception();
            }
            _filePath = path;
            StorageFolder = storageFolder;
            StorageFile = storageFile;
            ReadingInfo = info;
        }

        /// <summary>
        /// 返回BitmapImage，作为Image控件的source
        /// </summary>
        /// <returns> </returns>
        public async Task SetCover ()
        {
            StorageFolder coverfolder = await GetChildTemporaryFolder(nameof(Covers));

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

        //TODO 放到外面去
        /// <summary> 确保生成封面 </summary>
        /// <returns> </returns>
        public async Task EnsureCoverFile (StorageFile storageFile)
        {
            StorageFolder folder = await GetChildTemporaryFolder(nameof(Covers));
            IStorageItem storageItem = await folder.TryGetItemAsync(storageFile.DisplayName + ".jpg");
            if (storageItem is null)
            {
                await CoverHelper.CreatOriginCoverFile_UsingZipArchiveEntry(storageFile);

                // 这段代码有坑 在debug模式下，这个try -
                // catch块可以正常运行，在release模式下无法运行
                // SkiaSharp库存在Bug，某些正常图片无法解码

                ////try
                ////{
                ////    await CoverHelper.CreatThumbnailCoverFile_UsingSkiaSharp(storageFile);
                ////}
                ////catch (Exception ex)
                ////{
                ////    IStorageItem storageItem1 = await folder.TryGetItemAsync(storageFile.DisplayName + ".jpg");

                ////    await storageItem1?.DeleteAsync(StorageDeleteOption.PermanentDelete);

                ////    await CoverHelper.CreatOriginCoverFile_UsingZipArchiveEntry(storageFile);
                ////}
            }
        }
    }
}