using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Models;

namespace UnoApp;

public partial class RemoteMangaViewModel(MangaAPIClient _client) : ObservableObject
{
    public ObservableCollection<Manga> Mangas { get; } = [];
    public ObservableCollection<MangasGroupDTO> Groups { get; } = [];

    [ObservableProperty]
    public partial int DisplayAmount { set; get; } = 5;
    public List<int> DisplayAmountOptions = [5, 10, 20];
    public int MaxPageIndex;

    [ObservableProperty]
    public partial MangasGroupDTO SelectedGroup { set; get; }

    async partial void OnDisplayAmountChanged(int value)
    {
        await UpdateSelectedGroupRandomManga();
    }

    async partial void OnSelectedGroupChanged(MangasGroupDTO value)
    {
        await UpdateMangaCountPageAmount(SelectedGroup);
    }

    /// <summary>
    /// 异步下载封面到MemoryStream，转换为base64字符串，赋值给manga.CoverUri，并将 manga 添加到 Mangas 集合中
    /// 在xaml中通过base64toimageconverter值转换器将base64字符串转换为ImageSource显示
    /// </summary>
    private async Task LoadSetCover_Base64String_AddMangaAsync(Manga manga)
    {
        // 1. 下载到内存
        using MemoryStream? memoryStream = new();
        using var httpStream = await _client.GetCoverStreamAsync(manga.Guid);
        await httpStream.CopyToAsync(memoryStream);

        //2.转为 Base64 字符串（纯.NET 操作，无需 UI 线程）
        manga.CoverUri = Convert.ToBase64String(memoryStream.ToArray());
        ////直接用 Data URI 协议
        //manga.CoverUri = $"data:image/jpeg;base64,{Convert.ToBase64String(memoryStream.ToArray())}";
        Mangas.Add(manga);
    }

    /// <summary>
    /// 异步下载封面到本地临时文件，设置路径并将 manga 添加到 Mangas 集合中
    /// </summary>
    /// <param name="manga"></param>
    /// <returns></returns>
    [Obsolete("使用 LoadSetCover_Base64String_AddMangaAsync，避免IO读写")]
    private async Task LoadSetCover_File_AddMangaAsync(Manga manga)
    {
        // 1. 异步创建临时文件
        var storageFile = await ApplicationData
            .Current.TemporaryFolder.CreateFileAsync(
                $"{manga.Guid}.jpg",
                CreationCollisionOption.ReplaceExisting
            )
            .AsTask()
            .ConfigureAwait(false); // 库代码/非UI逻辑建议加上，避免不必要的上下文切换

        // 2. 异步下载并写入文件
        using var stream = await storageFile.OpenStreamForWriteAsync().ConfigureAwait(false);
        using var httpStream = await _client.GetCoverStreamAsync(manga.Guid).ConfigureAwait(false);
        await httpStream.CopyToAsync(stream).ConfigureAwait(false);

        // 3. 回到 UI 线程更新属性和集合（Uno/WinUI 必须）
        // 如果此方法在 UI 线程调用，去掉 ConfigureAwait(false) 即可自动回到 UI 线程
        manga.CoverUri = storageFile.Path;
        Mangas.Add(manga);
    }

    public async Task GetGroups()
    {
        var folders = await _client.GetGroupsBasicAsync();
        Groups.Clear();
        if (folders is not null)
        {
            foreach (var group in folders)
            {
                Groups.Add(group);
            }
        }
    }

    public async Task SelectFirst()
    {
        var group = Groups.FirstOrDefault();
        if (group is not null)
        {
            SelectedGroup = group;
        }
    }

    public async Task OpenAsync(Manga manga)
    {
#if ANDROID
        // 直接下载到文件
        var storagefile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(
            $"{manga.Name}.zip",
            Windows.Storage.CreationCollisionOption.ReplaceExisting
        );
        using var stream = await storagefile.OpenStreamForWriteAsync();
        var file = await _client.GetMangaStreamAsync(manga.Guid);
        await file.CopyToAsync(stream);

        AndroidOperation.Open(storagefile.Path);
#endif
    }

    [RelayCommand]
    public async Task Delete(Manga manga)
    {
        Mangas.Remove(manga);

        var a = await _client.DeleteAsync(manga.Guid);

        if (a.IsSuccessStatusCode)
        {
            await this.UpdateMangaCountPageAmount(SelectedGroup);
        }
    }

    public async Task UpdateMangaCount(MangasGroupDTO groupGuid)
    {
        groupGuid.Count = await _client.GetMangasCountAsync(groupGuid.Guid);
    }

    public async Task UpdateMangaCountPageAmount(MangasGroupDTO groupGuid)
    {
        await UpdateMangaCount(groupGuid);

        MaxPageIndex = (groupGuid.Count + DisplayAmount - 1) / DisplayAmount;
    }

    /// <summary>
    /// 获取指定group的指定起始索引的manga集合，并更新Mangas集合
    /// </summary>
    /// <param name="guid"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public async Task UpdateCollection(MangasGroupDTO guid, int index)
    {
        var mangas = await _client.GetSequenceMangasAsync(guid.Guid, index, DisplayAmount);
        Mangas.Clear();
        foreach (var m in mangas!)
        {
            await LoadSetCover_Base64String_AddMangaAsync(m);
        }
    }

    public async Task UpdateSelectedGroupRandomManga()
    {
        if (SelectedGroup is null)
        {
            return;
        }
        await UpdateMangaCount(SelectedGroup);
        Mangas.Clear();
        for (var i = 0; i < DisplayAmount; i++)
        {
            var random =
                SelectedGroup.Count < DisplayAmount
                    ? 0
                    : Random.Shared.Next(SelectedGroup.Count - 1);
            var mangas = await _client.GetSequenceMangasAsync(SelectedGroup.Guid, random, 1);

            var manga = mangas.First();
            await LoadSetCover_Base64String_AddMangaAsync(manga);
        }
    }

    // 3. 搜索/过滤方法
    [RelayCommand]
    public async Task SearchByTagAsync(string tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            return;
        Mangas.Clear();
        // 方式 B：重新从后端/数据库拉取（推荐，适合数据量大的情况）
        var result = _client.GetMangasByTagAsync(tagName);
        await foreach (var manga in result)
        {
            await LoadSetCover_Base64String_AddMangaAsync(manga);
        }
        //Mangas = new ObservableCollection<Manga>(result!);
    }
}
