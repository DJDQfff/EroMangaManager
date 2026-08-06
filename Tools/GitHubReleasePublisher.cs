using Octokit;

namespace Tools;

public class GitHubReleasePublisher
{
    private readonly GitHubClient _client;
    private readonly string _owner;
    private readonly string _repo;

    public GitHubReleasePublisher(string owner, string repo)
    {
        _owner = owner;
        _repo = repo;

        var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        if (string.IsNullOrEmpty(token))
            throw new InvalidOperationException("环境变量 GITHUB_TOKEN 未设置");

        _client = new GitHubClient(new ProductHeaderValue("EroMangaManager"))
        {
            Credentials = new Credentials(token),
        };
    }

    public async Task PublishAsync(string version, List<string> files)
    {
        // 1. 等待网络连接
        await WaitForConnectionAsync();

        // 2. 过滤存在的文件
        var validFiles = files.Where(File.Exists).ToList();
        if (validFiles.Count == 0)
            throw new InvalidOperationException("没有有效的文件可上传");

        // 3. 创建 Release（处理 tag 冲突），失败全流程重试
        var release = await CreateReleaseWithRetryAsync(version);

        // 4. 上传文件，失败全流程重试
        await UploadFilesWithRetryAsync(release, validFiles);

        Console.WriteLine($"✅ 发布完成: v{version}");
    }

    private async Task WaitForConnectionAsync()
    {
        while (true)
        {
            try
            {
                var user = await _client.User.Current();
                Console.WriteLine($"✅ 连接成功: {user.Login}");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ 连接失败: {ex.Message}，60秒后重试...");
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }
    }

    private async Task<Release> CreateReleaseWithRetryAsync(string version)
    {
        while (true)
        {
            try
            {
                var tag = await GenerateUniqueTagAsync(version);
                var newRelease = new NewRelease(tag)
                {
                    Name = $"Release {tag}",
                    Body = $"版本 {tag}",
                    Draft = false,
                    Prerelease = false,
                };

                var release = await _client.Repository.Release.Create(_owner, _repo, newRelease);
                Console.WriteLine($"✅ Release 创建成功: {tag}");
                return release;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 创建 Release 失败: {ex.Message}，重新开始流程...");
                await Task.Delay(5000);
            }
        }
    }

    private async Task UploadFilesWithRetryAsync(Release release, List<string> files)
    {
        while (true)
        {
            try
            {
                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file);
                    Console.WriteLine($"  上传: {fileName}");

                    using var stream = File.OpenRead(file);
                    var upload = new ReleaseAssetUpload
                    {
                        FileName = fileName,
                        ContentType = file.EndsWith(".apk")
                            ? "application/vnd.android.package-archive"
                            : "application/octet-stream",
                        RawData = stream,
                    };

                    await _client.Repository.Release.UploadAsset(release, upload);
                }

                Console.WriteLine("✅ 所有文件上传完成");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 上传失败: {ex.Message}");

                // 删除失败的 Release，全流程重来
                try
                {
                    await _client.Repository.Release.Delete(_owner, _repo, release.Id);
                }
                catch { }

                Console.WriteLine("重新开始发布流程...");
                await Task.Delay(5000);

                // 重走创建 + 上传
                var newRelease = await CreateReleaseWithRetryAsync(release.TagName);
                release = newRelease;
            }
        }
    }

    private async Task<string> GenerateUniqueTagAsync(string version)
    {
        // 去除每一段的前导零，例如 "2026.05.15" -> "2026.5.15"
        var parts = version
            .Split('.')
            .Select(p => p.TrimStart('0').Length > 0 ? p.TrimStart('0') : "0");
        var cleanVersion = string.Join(".", parts);

        var baseTag = $"v{cleanVersion}";
        var tag = baseTag;
        var suffix = 2;

        while (true)
        {
            try
            {
                await _client.Git.Reference.Get(_owner, _repo, $"tags/{tag}");
                tag = $"{baseTag}-{suffix}";
                suffix++;
            }
            catch (NotFoundException)
            {
                return tag;
            }
        }
    }
}
