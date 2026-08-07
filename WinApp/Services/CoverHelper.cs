using Core.Services;
using Database.Entities;
using Microsoft.UI.Xaml.Media.Imaging;
using SharpCompress.Archives;
using static Core.Setting.FolderEnum;

namespace UnoLibrary.Services;

/// <summary>
/// 封面帮助类
/// </summary>
public class CoverHelper
{
    private readonly SvgImageSource _imageSource;

    private readonly SvgImageSource _errorimageSource;

    private readonly StorageFolderHelper storageFolderHelper;

    public CoverHelper(StorageFolderHelper _storageFolderHelper)
    {
        _imageSource = new(new(DefaultCoverUri));
        _errorimageSource = new(new(ErrorCoverUri));
        storageFolderHelper = _storageFolderHelper;
    }

    /// <summary>
    /// 默认书籍封面路径
    /// </summary>
    public string DefaultCoverUri { get; } = "ms-appx:///Assets/SVGs/book.svg";

    public string ErrorCoverUri { get; } = "ms-appx:///Assets/SVGs/book-wrong-fill.svg";

    /// <summary>
    /// 获取默认封面
    /// </summary>
    public SvgImageSource DefaultCoverImage => _imageSource;

    public SvgImageSource ErrorCoverImage => _errorimageSource;

    ///// <summary> 调用系统API，返回缩率图 </summary>
    ///// <param name="cover"> </param>
    ///// <returns> </returns>
    //public async Task<BitmapImage> GetCoverThumbnail_SystemAsync (StorageFile cover)
    //{
    //    BitmapImage bitmapImage = new();
    //    //thumbnailMode.picturemode有坑,缩略图不完整

    //    using var thumbnail = await cover.GetThumbnailAsync(ThumbnailMode.SingleItem , 80);

    //    IRandomAccessStream randomAccessStream = thumbnail.CloneStream();

    //    await bitmapImage.SetSourceAsync(randomAccessStream);

    //    return bitmapImage;
    //}

    /// <summary>
    /// 清除所有封面文件
    /// </summary>
    /// <returns></returns>
    public async Task ClearCovers()
    {
        StorageFolder storageFolder = await storageFolderHelper.GetChildTemporaryFolder(
            nameof(Covers)
        );
        var coverfolder = storageFolder.Path;
        Directory.Delete(coverfolder, true);
        storageFolderHelper.EnsureTemporaryFolderChild_Covers(Covers.ToString());
    }

    /// <summary> 尝试创建封面文件。 </summary>
    /// <returns> </returns>
    public async Task<string> TryCreatCoverFileAsync(
        string storageFile,
        FilteredImage[]? filteredImages
    )
    {
        var folder = await storageFolderHelper.GetChildTemporaryFolder(nameof(Covers));
        var coverfile = Path.Combine(Path.GetFileNameWithoutExtension(storageFile), ".jpg");

        var storageItem = await folder.TryGetItemAsync(coverfile);

        if (storageItem is null)
        {
            return await CreatCoverFile_Origin_SharpCompress(storageFile, filteredImages)
                .ConfigureAwait(false);
        }
        else
        {
            // 命中缓存时，ValueTask 会将结果存在栈上的结构体中，零堆分配！
            return storageItem.Path;
        }
    }

    /// <summary>
    /// 使用SharpCompress类库创建源图片设为封面
    /// </summary>
    /// <param name="storageFile"></param>
    /// <param name="filteredImages">要比较的数据</param>
    /// <returns></returns>
    public async Task<string> CreatCoverFile_Origin_SharpCompress(
        string storageFile,
        FilteredImage[]? filteredImages
    )
    {
        string path = null!;
        var coverfolder = await storageFolderHelper.GetChildTemporaryFolder(nameof(Covers));
        var stream = new FileStream(storageFile, FileMode.Open, FileAccess.Read, FileShare.Read);

        using (var zipArchive = ArchiveFactory.OpenArchive(stream))
        {
            foreach (var entry in zipArchive.Entries)
            {
                bool canuse = entry.EntryFilter(filteredImages);
                if (canuse)
                {
                    path = Path.Combine(
                        coverfolder.Path,
                        Path.GetFileNameWithoutExtension(storageFile) + ".jpg"
                    );
                    entry.WriteToFile(path);
                    break;
                }
            }
        }
        return path;
    }
}
