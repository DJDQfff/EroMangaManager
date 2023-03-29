using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

using EroMangaDB.Helper;

namespace EroMangaManager.Core.Models
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
        public string FilePath { get; private set; }


        /// <summary> 获取文件的扩展名 </summary>
        public string FileExtension => Path.GetExtension(FilePath).ToLower();

        /// <summary> 文件Display名（不带扩展名） </summary>
        public string FileDisplayName => Path.GetFileNameWithoutExtension(FilePath);

        /// <summary>
        /// 获取漫画文件大小。单位：字节
        /// </summary>
        public ulong FileSize { get; set; }

        /// <summary> 漫画文件所在文件夹路径 </summary>
        public string FolderPath => Path.GetDirectoryName(FilePath);

        /// <summary> 漫画文件名（全名，带扩展名，不包含文件夹名） </summary>
        public string FileFullName => Path.GetFileName(FilePath);



        private string manganame;
        /// <summary> 本子名字 </summary>
        public virtual string MangaName { get=>manganame; set { manganame = value;NotifyPropertyChanged(); } }

        /// <summary> 包含在文件名中的本子Tag </summary>
        public string[] MangaTagsIncludedInFileName { get; }

        /// <summary> 漫画翻译后的断名称 </summary>
        public string TranslatedMangaName { set; get; }

        //TODO 翻译漫画名的功能

        // TODO 漫画初始化，这个工作放到平台相关类里实现
        /// <summary> 实例化EroManga </summary>
        /// <param name="storageFile"> </param>
        /// <param name="storageFolder">所属文件夹</param>
        public MangaBook(string filepath)
        {
            FilePath = filepath;

            var tags = FileDisplayName.SplitAndParser();
            MangaName = tags[0];
            MangaTagsIncludedInFileName = tags.Skip(1).ToArray();
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}