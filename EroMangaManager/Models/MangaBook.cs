﻿using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

using EroMangaDB.Helper;

using Windows.Storage;
using Windows.UI.Xaml.Media;

namespace EroMangaManager.Models
{
    /// <summary> 本子 </summary>
    public class MangaBook : INotifyPropertyChanged
    {

        /// <summary>
        /// 封面文件路径
        /// </summary>
        /// <remarks>这个一定要有，不能为null，不然在Image控件加载图像时会异常导致程序闪退</remarks>
        public string CoverPath { set; get; }
        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary> 漫画文件路径 </summary>
        public string FilePath { get; }

        /// <summary> 获取文件的扩展名 </summary>
        public string FileExtension => Path.GetExtension(FilePath).ToLower();

        /// <summary> 文件Display名（不带扩展名） </summary>
        public string FileDisplayName => Path.GetFileNameWithoutExtension(FilePath);

        /// <summary>
        /// 获取漫画文件大小。单位：字节
        /// </summary>
        public ulong FileSize => StorageFile.GetBasicPropertiesAsync().AsTask().Result.Size;

        /// <summary> 漫画所在文件夹路径 </summary>
        public string FolderPath => Path.GetDirectoryName(FilePath);

        /// <summary> 漫画文件名（全名，带扩展名，不包含文件夹名） </summary>
        public string FileFullName => Path.GetFileName(FilePath);

        /// <summary> 本子对应具体文件 </summary>
        public StorageFile StorageFile { get; }

        /// <summary> 本子所属文件夹 </summary>
        public StorageFolder StorageFolder { set; get; }

        /// <summary> 本子名字 </summary>
        public virtual string MangaName { get; }

        /// <summary> 包含在文件名中的本子Tag </summary>
        public string[] MangaTagsIncludedInFileName { get; }

        /// <summary> 漫画翻译后的断名称 </summary>
        public string TranslatedMangaName { set; get; }

        //TODO 翻译漫画名的功能

        /// <summary> 实例化EroManga </summary>
        /// <param name="storageFile"> </param>
        /// <param name="storageFolder">所属文件夹</param>
        public MangaBook (StorageFile storageFile , StorageFolder storageFolder)
        {
            string path = storageFile.Path;
            FilePath = path;
            StorageFolder = storageFolder;
            StorageFile = storageFile;

            var tags = FileDisplayName.SplitAndParser();
            MangaName = tags[0];
            MangaTagsIncludedInFileName = tags.Skip(1).ToArray();
        }

        private void NotifyPropertyChanged ([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this , new PropertyChangedEventArgs(propertyName));
        }
    }
}