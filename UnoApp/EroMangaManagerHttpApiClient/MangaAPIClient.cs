using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using EroMangaManager.Core.DTOs;
using EroMangaManager.Core.Models;
using ZstdSharp.Unsafe;

namespace EroMangaManagerHttpApiClient;

public class MangaAPIClient
{
    HttpClient client;
    public MangaAPIClient()
    {
        client = new HttpClient
        {
            BaseAddress = new Uri("http://192.168.2.108:12965/")
        };
    }

    public MangaAPIClient(string baseUrl)
    {
        if (!baseUrl.EndsWith("/"))
            baseUrl += "/";
        client = new HttpClient
        {
            BaseAddress = new Uri(baseUrl)
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
    public async Task<IEnumerable<MangasGroupDTO>> GetGroupsBasicAsync()
    {
        return await client.GetFromJsonAsync<IEnumerable<MangasGroupDTO>>("/folders/basicinfo");

    }
    public async Task<Stream> GetStreamAsync(string mangaGuid)
    {
        return await client.GetStreamAsync($"/downloads/{mangaGuid}");
    }
    public async Task<HttpResponseMessage> DeleteAsync(string mangaGuid)
    {
        return await client.DeleteAsync($"/mangas/{mangaGuid}");
    }
    public async IAsyncEnumerable<MangaDTO> GetManyMangaDTOsAsync(string groupGuid, int skip, int take)
    {

        await foreach (var mangaDTO in client.GetFromJsonAsAsyncEnumerable<MangaDTO>($"/folders/{groupGuid}/{skip}/{take}"))
        {
            mangaDTO.CoverUri = $"{client.BaseAddress}covers/{mangaDTO.Guid}";
            yield return mangaDTO; 
        }
        
    }
    public async Task<int> GetMangasCountAsync(string groupGuid) {
        var countstr = await client.GetStringAsync($"/folders/{groupGuid}/count");
        var count = int.Parse(countstr);
        return count;
    }

}
