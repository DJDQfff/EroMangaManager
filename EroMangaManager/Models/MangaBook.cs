﻿using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using EroMangaDB.Entities;
using EroMangaDB.Helper;
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
        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private ImageSource imagesource;

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

        /// <summary>
        /// 获取漫画文件大小。单位：字节
        /// </summary>
        public ulong FileSize => StorageFile.GetBasicPropertiesAsync().AsTask().Result.Size;
        /// <summary> 漫画所在文件夹路径 </summary>
        public string FolderPath => Path.GetDirectoryName(_filePath);

        /// <summary> 漫画文件名（全名，带扩展名） </summary>
        public string FileName => Path.GetFileName(_filePath);

        /// <summary> 本子对应具体文件 </summary>
        public StorageFile StorageFile { get; }

        /// <summary> 本子所属文件夹 </summary>
        public StorageFolder StorageFolder { set; get; }

        /// <summary> 本子名字 </summary>
        public virtual string MangaName =>( FileDisplayName.SplitAndParser())[0];

        /// <summary> 本子Tag </summary>
        public string[] MangaTags 
        {
            get
            {
                var a = FileDisplayName.SplitAndParser();
                a.RemoveAt(0);
                return a.ToArray();
    }
     }
        // TODO

        /// <summary> 漫画翻译后的断名称 </summary>
        public string TranslatedMangaName { set; get; }
        //TODO
         
        /// <summary> 实例化EroManga </summary>
        /// <param name="storageFile"> </param>
        /// <param name="storageFolder">所属文件夹</param>
        public MangaBook (StorageFile storageFile , StorageFolder storageFolder )
        {
            string path = storageFile.Path;
            _filePath = path;
            StorageFolder = storageFolder;
            StorageFile = storageFile;
        }
        private void NotifyPropertyChanged ([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this , new PropertyChangedEventArgs(propertyName));
        }
    }
}