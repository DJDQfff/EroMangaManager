namespace Tools;

public class ApkPackager
{
    public List<string> Files { get; } = [];

    readonly string rootPath;
    readonly string version;
    readonly string publishversionfolder;
    readonly string slnPath;

    readonly string Configuration = "Release";
    readonly string AndroidTargetFramwork = "net10.0-android";
    readonly string[] AndroidRuntimeIdentifiers = ["android-arm" , "android-arm64"];

    public ApkPackager (string _version , string slnFolder)
    {
        rootPath = slnFolder;
        slnPath = Directory.GetFiles(rootPath , "*.slnx").First();
        version = _version;

        publishversionfolder = Path.Combine(rootPath , "publish" , version);
        Directory.CreateDirectory(publishversionfolder);
    }

    public void CleanThenRestoreSlnx ()
    {
        if (!string.IsNullOrEmpty(slnPath))
        {
            PackagingUtils.Run("dotnet" , $"clean \"{slnPath}\"" , rootPath);
            PackagingUtils.Run("dotnet" , $"restore \"{slnPath}\"" , rootPath);
        }
    }

    public void PublishAPK ()
    {
        string unoappcsproj = Path.Combine(rootPath , "UnoApp/UnoApp.csproj");
        string unorelease = Path.Combine(
            rootPath ,
            $"UnoApp/bin/{Configuration}/{AndroidTargetFramwork}"
        );

        foreach (var runtime in AndroidRuntimeIdentifiers)
        {
            try
            {
                var args =
                    $" publish \"{unoappcsproj}\""
                    + $" -f {AndroidTargetFramwork}"
                    + $" -r {runtime}"
                    + $" -c {Configuration}";

                PackagingUtils.Run("dotnet" , args , rootPath);

                var folder = Path.Combine(unorelease , runtime , "publish");
                var apks = Directory.EnumerateFiles(
                    folder ,
                    $"*-Signed.apk" ,
                    new EnumerationOptions() { RecurseSubdirectories = true }
                );

                var apk = apks.Single();
                var newapk = Path.Combine(
                    publishversionfolder ,
                    Path.GetFileNameWithoutExtension(apk) + $"-{runtime}.apk"
                );

                File.Move(apk , newapk , true);
                Files.Add(newapk);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}