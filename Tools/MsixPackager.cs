using System.Reflection.PortableExecutable;

namespace Tools;

public class MsixPackager
{
    public List<string> Files { get; } = [];

    readonly string rootPath;
    readonly string version;
    readonly string msixbundleFIle;
    readonly string publishversionfolder;
    readonly string slnPath;

    readonly string MSBuildExe = @"C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe";
    readonly string Configuration = "Release";
    readonly string WindowsTargetFrameWork = "net10.0-windows10.0.26100";
    readonly string[] WindowsPlatforms = ["x64" , "x86" , "ARM64"];
    readonly Dictionary<string , string> WindowsRuntimeIdentifiers = new()
    {
        ["x64"] = "win-x64" ,
        ["x86"] = "win-x86" ,
        ["ARM64"] = "win-arm64" ,
    };

    public MsixPackager (string _version , string slnFolder)
    {
        rootPath = slnFolder;
        slnPath = Directory.GetFiles(rootPath , "*.slnx").First();
        version = _version;

        msixbundleFIle = Path.Combine(rootPath , "publish" , $"{version}.msixbundle");
        publishversionfolder = Path.Combine(rootPath , "publish" , version);

        Directory.CreateDirectory(publishversionfolder);
        Files.Add(Path.Combine(rootPath , "DJDQfff_certificate.cer"));
    }

    public void CleanThenRestoreSlnx ()
    {
        if (!string.IsNullOrEmpty(slnPath))
        {
            PackagingUtils.Run("dotnet" , $"clean \"{slnPath}\"" , rootPath);
            PackagingUtils.Run("dotnet" , $"restore \"{slnPath}\"" , rootPath);
        }
    }

    public void BuildMsix ()
    {
        // 寻找打包和签名工具
        var packtoolfolder = Directory
            .GetDirectories(
                "C:\\Program Files (x86)\\Microsoft Visual Studio\\Shared\\NuGetPackages\\microsoft.windows.sdk.buildtools\\" ,
                "x64" ,
                new EnumerationOptions() { RecurseSubdirectories = true }
            )
            .First();

        var makeappx = Directory.GetFiles(packtoolfolder).Single(x => x.EndsWith("makeappx.exe"));
        var signtool = Directory.GetFiles(packtoolfolder).Single(x => x.EndsWith("signtool.exe"));

        string winappcsproj = Path.Combine(rootPath , "WinApp/WinApp.csproj");
        string WinApp_bin = Path.Combine(rootPath , "WinApp/bin");

        foreach (var platform in WindowsPlatforms)
        {
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
                    + $" /p:PackageCertificateKeyFile={PackagingUtils.PfxFile}"
                    + $" /p:PackageCertificatePassword={PackagingUtils.GetPfxPassword()}";

                PackagingUtils.Run(MSBuildExe , msbuildArgs , rootPath);

                var folder = Path.Combine(
                    WinApp_bin ,
                    platform ,
                    Configuration ,
                    WindowsTargetFrameWork ,
                    WindowsRuntimeIdentifiers[platform]
                );

                var files = Directory.EnumerateFiles(
                    folder ,
                    $"*_{platform}.msix" ,
                    new EnumerationOptions() { RecurseSubdirectories = true }
                );
                var msix = files.Single();
                var target = Path.Combine(publishversionfolder , $"{Path.GetFileName(msix)}");
                File.Move(msix , target , true);
                Files.Add(target);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        // 打包 Bundle 并签名
        PackagingUtils.Run(makeappx , $"bundle /o /d \"{publishversionfolder}\" /p \"{msixbundleFIle}\"" , rootPath);
        PackagingUtils.Run(signtool , $"sign /fd SHA256 /a /f \"{PackagingUtils.PfxFile}\" /p {PackagingUtils.GetPfxPassword()} \"{msixbundleFIle}\"" , rootPath);
        Files.Add(msixbundleFIle);
    }
}