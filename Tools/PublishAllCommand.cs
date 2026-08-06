using System.Diagnostics;

namespace Tools;

public class DotnetMakePackages
{
    public List<string> Files { get; } = [];
    readonly string rootPath;
    readonly string pfxPath;
    readonly string PfxPassword =
        Environment.GetEnvironmentVariable("MADAO_PASSWORD")
        ?? throw new ArgumentException("环境变量：证书密码未配置");

    readonly string version;
    readonly string packtoolfolder;
    readonly string makeappx;
    readonly string signtool;
    readonly string msixbundleFIle;
    readonly string publishversionfolder;
    readonly string slnPath;
    readonly string MSBuildExe =
        @"C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe";
    readonly string Configuration = "Release";
    readonly string AndroidTargetFramwork = "net10.0-android";

    readonly string WindowsTargetFrameWork = "net10.0-windows10.0.26100";
    readonly string[] WindowsPlatforms = ["x64", "x86", "ARM64"];
    readonly Dictionary<string, string> WindowsRuntimeIdentifiers = new()
    {
        ["x64"] = "win-x64",
        ["x86"] = "win-x86",
        ["ARM64"] = "win-arm64",
    };
    readonly string[] AndroidRuntimeIdentifiers = ["android-arm", "android-arm64"];

    public DotnetMakePackages(string _version, string slnFolder)
    {
        rootPath = slnFolder;
        slnPath = Directory.GetFiles(rootPath, "*.slnx").First();

        // ... 其他原有代码保持不变
        version = _version;
        msixbundleFIle = Path.Combine(rootPath, "publish", $"{version}.msixbundle");

        packtoolfolder = Directory
            .GetDirectories(
                "C:\\Program Files (x86)\\Microsoft Visual Studio\\Shared\\NuGetPackages\\microsoft.windows.sdk.buildtools\\",
                "x64",
                new EnumerationOptions() { RecurseSubdirectories = true }
            )
            .First();
        pfxPath = Path.Combine(rootPath, "DJDQfff_new.pfx");

        makeappx = Directory.GetFiles(packtoolfolder).Single(x => x.EndsWith("makeappx.exe"));
        signtool = Directory.GetFiles(packtoolfolder).Single(x => x.EndsWith("signtool.exe"));

        publishversionfolder = Path.Combine(slnFolder, "publish", version);
        Directory.CreateDirectory(publishversionfolder);
        Files.Add(Path.Combine(rootPath, "DJDQfff_certificate.cer"));
    }

    public void BuildMsix()
    {
        string winappcsproj = Path.Combine(rootPath, "WinApp/WinApp.csproj");
        string WinApp_bin = Path.Combine(rootPath, "WinApp/bin");
        foreach (var platform in WindowsPlatforms)
        {
            //var tempDirectory = Directory.CreateTempSubdirectory().FullName;
            //Process.Start("dotnet" ,
            //    $" restore {winappcsproj}" +
            //    $" -p:TargetFramework={targetframwork}")
            //    .WaitForExit();
            //Process.Start("dotnet" ,
            //    $" build {winappcsproj}" +
            //    $" --no-restore" +
            //    $" --no-dependencies" +
            //    $" -c Release" +
            //    $" -f {targetframwork} " +
            //    $" -p:Platform={platform} " +
            //    $" -p:RuntimeIdentifier={WindowsRuntimeIdentifiers[platform]}" +
            //    $" -p:AppxBundle=Never " +
            //    $" /p:GenerateAppxPackageOnBuild=true " +
            //    $" /p:PackageCertificateKeyFile={pfxPath} " +
            //    $" /p:PackageCertificatePassword={PfxPassword}")
            //    .WaitForExit();
            try
            {
                string msbuildArgs =
                    $"{winappcsproj} /t:Publish"
                    + $" /p:Configuration={Configuration}"
                    + $" /p:Platform={platform}"
                    + $" /p:RuntimeIdentifier={WindowsRuntimeIdentifiers[platform]}"
                    + $" /p:TargetFramework={WindowsTargetFrameWork}"
                    + $" /p:AppxBundle=Never"
                    + $" /p:GenerateAppxPackageOnBuild=true"
                    + $" /p:PackageCertificateKeyFile={pfxPath}"
                    + $" /p:PackageCertificatePassword={PfxPassword}";

                Run(MSBuildExe, msbuildArgs, rootPath);
                var folder = Path.Combine(
                    WinApp_bin,
                    platform,
                    Configuration,
                    WindowsTargetFrameWork,
                    WindowsRuntimeIdentifiers[platform]
                );
                var files = Directory.EnumerateFiles(
                    folder,
                    $"*_{platform}.msix",
                    new EnumerationOptions() { RecurseSubdirectories = true }
                );
                var msix = files.Single();
                var target = Path.Combine(publishversionfolder, $"{Path.GetFileName(msix)}");
                File.Move(msix, target, true);

                Files.Add(target);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // 👇 3. 打包和签名也换成 Run 方法
        Run(makeappx, $"bundle /o /d \"{publishversionfolder}\" /p \"{msixbundleFIle}\"", rootPath);
        Run(
            signtool,
            $"sign /fd SHA256 /a /f \"{pfxPath}\" /p {PfxPassword} \"{msixbundleFIle}\"",
            rootPath
        );
        Files.Add(msixbundleFIle);
    }

    public void CleanThenRestoreSlnx()
    {
        if (!string.IsNullOrEmpty(slnPath))
        {
            // 1. 先清理
            Run("dotnet", $"clean \"{slnPath}\"", rootPath);

            // 2. 再还原（确保后续构建不会报错）
            Run("dotnet", $"restore \"{slnPath}\"", rootPath);
        }
    }

    public void PubllishAPK()
    {
        string unoappcsproj = Path.Combine(rootPath, "UnoApp/UnoApp.csproj");

        string unorelease = Path.Combine(
            rootPath,
            $"UnoApp/bin/{Configuration}/{AndroidTargetFramwork}"
        );

        foreach (var runtime in AndroidRuntimeIdentifiers)
        {
            try
            {
                //var tempDirectory = Directory.CreateTempSubdirectory().FullName;
                // 发布 APK
                var args =
                    $" publish \"{unoappcsproj}\""
                    + $" -f {AndroidTargetFramwork}"
                    + $" -r {runtime}"
                    + $" -c {Configuration}";
                Run("dotnet", args, rootPath);

                var folder = Path.Combine(unorelease, runtime, "publish");
                var apks = Directory.EnumerateFiles(
                    folder,
                    $"*-Signed.apk",
                    new EnumerationOptions() { RecurseSubdirectories = true }
                );
                var apk = apks.Single();
                var newapk = Path.Combine(
                    publishversionfolder,
                    Path.GetFileNameWithoutExtension(apk) + $"-{runtime}.apk"
                );
                File.Move(apk, newapk, true);
                Files.Add(newapk);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    [Obsolete]
    void CleanBuildDirectories()
    {
        string[] bin_obj = ["bin", "obj"];
        string[] folders = ["WinApp", "Database", "Server", "Core", "UnoLibrary", "UnoApp"];
        foreach (var folder in folders)
        {
            var directoryinfo = new DirectoryInfo(Path.Combine(rootPath, folder));

            foreach (var b_o in bin_obj)
            {
                var objfolder = directoryinfo.GetDirectories(b_o).SingleOrDefault();
                objfolder?.Delete(true);
            }
        }
    }

    static void Run(string fileName, string arguments, string workingDir)
    {
        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            WorkingDirectory = workingDir,
            UseShellExecute = false,
            CreateNoWindow = false,
        };

        using var proc = Process.Start(psi);
        proc?.WaitForExit();
        if (proc?.ExitCode != 0)
        {
            throw new Exception(
                $"命令执行失败 (ExitCode: {proc?.ExitCode}): {fileName} {arguments}"
            );
        }
    }
}
