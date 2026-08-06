using Core.Models;
using Windows.Media.Protection.PlayReady;

namespace UnoApp;

public class MangaAPIClient
{
    readonly HttpClient client;
    public Uri BaseAddress => client.BaseAddress!;

    public MangaAPIClient()
    {
        client = new()
        {
            BaseAddress = new Uri("http://192.168.1.108:12965/"),
            Timeout = System.Threading.Timeout.InfiniteTimeSpan, // 永不超时
        };
    }

    public MangaAPIClient(string baseUrl)
    {
        if (!baseUrl.EndsWith('/'))
            baseUrl += "/";
        client = new HttpClient
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = System.Threading.Timeout.InfiniteTimeSpan, // 永不超时
        };
    }

    public async Task<bool> CheckConnectionAsync()
    {
        try
        {
            var response = await client.GetAsync("/api/health");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    // 1. 去掉 async 关键字
    // 2. 去掉 await
    // 3. 直接返回 Task
    public Task<IEnumerable<MangasGroupDTO>?> GetGroupsBasicAsync()
    {
        return client.GetFromJsonAsync<IEnumerable<MangasGroupDTO>>("/folders/basicinfo");
    }

    public Task<Stream> GetCoverStreamAsync(string mangaGuid)
    {
        return client.GetStreamAsync($"/covers/{mangaGuid}");
    }

    public Task<Stream> GetMangaStreamAsync(string mangaGuid)
    {
        return client.GetStreamAsync($"/downloads/{mangaGuid}");
    }

    public Task<HttpResponseMessage> DeleteAsync(string mangaGuid)
    {
        return client.DeleteAsync($"/Mangas/{mangaGuid}");
    }

    /// <summary>
    /// 获取指定group的mangas，从指定索引开始，获取指定数量
    /// </summary>
    /// <param name="groupGuid"></param>
    /// <param name="index"></param>
    /// <param name="take"></param>
    /// <returns></returns>
    public Task<IEnumerable<Manga>?> GetSequenceMangasAsync(string groupGuid, int index, int take)
    {
        if (index < 0)
        {
            index = 0;
        }
        var url = $"/folders/{groupGuid}/{index}/{take}";
        return client.GetFromJsonAsync<IEnumerable<Manga>>(url);
    }

    // 客户端 ApiClient 最优雅的写法
    public Task<int> GetMangasCountAsync(string groupGuid)
    {
        // 直接让 HttpClient 把 JSON 数字反序列化为 int，连 await 都省了
        return client.GetFromJsonAsync<int>($"/folders/{groupGuid}/count");
    }

    public async IAsyncEnumerable<Manga> GetMangasByTagAsync(string tag)
    {
        var uri = $"/mangas/with_tag/{tag}";
        await foreach (var manga in client.GetFromJsonAsAsyncEnumerable<Manga>(uri))
        {
            if (manga is not null)
                yield return manga;
        }
    }
}
