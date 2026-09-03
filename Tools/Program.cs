global using static System.Console;
using Tools;

var version = "2026.9.4";
var Slnx = "E:\\Projects\\EroMangaManager";
//var remoteSlnx= "E:\\Projects\\EroMangaManagerRemote";


// 执行 MSIX 打包
MsixPackager msixPackager = new (version , Slnx);
//msixPackager.CleanThenRestoreSlnx();
msixPackager.BuildMsix();

Console.WriteLine("MSIX 打包完成，生成文件：");
foreach (var file in msixPackager.Files)
{
    Console.WriteLine(file);
}

// 执行 APK 打包
ApkPackager apkPackager = new(version , Slnx);
//apkPackager.CleanThenRestoreSlnx();
apkPackager.PublishAPK();

Console.WriteLine("APK 打包完成，生成文件：");
foreach (var file in apkPackager.Files)
{
    Console.WriteLine(file);
}

List<string> files =[.. apkPackager.Files,..msixPackager.Files];

GitHubReleasePublisher publisher = new("DJDQfff" , "EroMangaManager");

await publisher.PublishAsync(version , files);


