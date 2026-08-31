using Core.Models;

namespace UnoApp;

public class MangaAPIClient
{
    HttpClient client = null!;
    public Uri BaseAddress => client.BaseAddress!;
    public ServerStorage ServerStorage;

    public MangaAPIClient (ServerStorage serverStorage)
    {
        ServerStorage = serverStorage;
        var baseUrl = serverStorage.LoadLastServer() ?? "http://0.0.0.0:12965";
        UpdateHttpClient(baseUrl);

    }
    public void UpdateHttpClient (string baseUrl)
    {
        if (!baseUrl.EndsWith('/'))
            baseUrl += "/";
        client = new HttpClient
        {
            BaseAddress = new Uri(baseUrl) ,
            Timeout = TimeSpan.FromSeconds(5) ,
        };
    }

    public async Task<bool> CheckConnectionAsync ()
    {
        var response = await client.GetAsync("/api/health");
        return response.IsSuccessStatusCode;
    }

    // 1. 去掉 async 关键字
    // 2. 去掉 await
    // 3. 直接返回 Task
    public Task<IEnumerable<MangasGroupDTO>?> GetGroupsBasicAsync ()
    {
        return client.GetFromJsonAsync<IEnumerable<MangasGroupDTO>>("/folders/basicinfo");
    }
    // 修改返回类型为 string，因为它只是生成一个 URL 字符串，不是异步操作
    public string GetCoverUri (string mangaGuid)
    {
        // 拼接完整的图片 URL，例如: http://192.168.1.108:12965/covers/file/{guid}
        return $"{client.BaseAddress}covers/file/{mangaGuid}";
    }
    public Task<Stream> GetCoverStreamAsync (string mangaGuid)
    {
        return client.GetStreamAsync($"/covers/file/{mangaGuid}");
    }
    // 修改方法签名，返回 byte[] 而不是 string
    public Task<byte[]> GetCoverBase64Async (string mangaGuid)
    {
        // 使用 GetByteArrayAsync 直接获取字节数组
        return client.GetByteArrayAsync($"/covers/base64/{mangaGuid}");
    }
    public Task<Stream> GetMangaStreamAsync (string mangaGuid)
    {
        return client.GetStreamAsync($"/downloads/{mangaGuid}");
    }

    public Task<HttpResponseMessage> DeleteAsync (string mangaGuid)
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
    public Task<IEnumerable<Manga>?> GetSequenceMangasAsync (string groupGuid , int index , int take)
    {
        if (index < 0)
        {
            index = 0;
        }
        var url = $"/folders/{groupGuid}/{index}/{take}";
        return client.GetFromJsonAsync<IEnumerable<Manga>>(url);
    }

    // 客户端 ApiClient 最优雅的写法
    public Task<int> GetMangasCountAsync (string groupGuid)
    {
        // 直接让 HttpClient 把 JSON 数字反序列化为 int，连 await 都省了
        return client.GetFromJsonAsync<int>($"/folders/{groupGuid}/count");
    }

    public async IAsyncEnumerable<Manga> GetMangasByTagAsync (string tag)
    {
        var uri = $"/mangas/with_tag/{tag}";
        await foreach (var manga in client.GetFromJsonAsAsyncEnumerable<Manga>(uri))
        {
            if (manga is not null)
                yield return manga;
        }
    }
}
