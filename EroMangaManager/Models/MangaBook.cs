using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using EroMangaDB.Entities;

/*
 * 简化版EroManga类
 * 只能获取本子名，是否无修，是否全彩、是否汉化、
 * 解压第一页图至应用临时文件夹下”PrimaryPicture"文件夹
 */

using EroMangaManager.Helpers;

using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

using static EroMangaManager.Models.FolderEnum;
using static MyUWPLibrary.StorageFolderHelper;

namespace EroMangaManager.Models
{
    /// <summary> 本子 </summary>
    public class MangaBook : INotifyPropertyChanged
    {
        /// <summary> 封面缩略图 </summary>
        //public BitmapImage CoverImage { private set; get; } = new BitmapImage();

        public event PropertyChangedEventHandler PropertyChanged;

        private ImageSource imagesource = new SvgImageSource(new Uri("ms-appx:///Assets/SVGs/书籍.svg"));

        /// <summary>
        /// 封面图像
        /// </summary>
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

        /// <summary>
        /// 当前阅读信息
        /// </summary>
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
        /// <param name="storageFolder">所属文件夹</param>
        /// <param name="info"></param>
        public MangaBook (StorageFile storageFile , StorageFolder storageFolder , ReadingInfo info)
        {
            string path = storageFile.Path;
            _filePath = path;
            StorageFolder = storageFolder;
            StorageFile = storageFile;
            ReadingInfo = info;
        }

        /// <summary>
        /// 返回BitmapImage，作为Image控件的source
        /// </summary>
        /// <returns> </returns>
        public async Task ChangeCover ()
        {
            StorageFolder coverfolder = await GetChildTemporaryFolder(nameof(Covers));

            var cover = await coverfolder.TryGetItemAsync(StorageFile.DisplayName + ".jpg");

            if (cover != null)
            {
                Windows.Storage.StorageFile storageFile = cover as Windows.Storage.StorageFile;
                BitmapImage bitmapImage = await CoverHelper.GetCoverThumbnail_SystemAsync(storageFile);

                Cover = bitmapImage;
            }
        }

        /// <summary>
        /// 翻译本子名
        /// </summary>
        /// <param name="name"></param>
        public void TranslateMangaName (string name)
        {
            this.MangaName = name;
        }

        private void NotifyPropertyChanged ([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this , new PropertyChangedEventArgs(propertyName));
        }
    }
}