global using static System.Console;
using Tools;

var version = "2026.8.10";
var Slnx = "E:\\Projects\\EroMangaManager";
var remoteSlnx= "E:\\Projects\\EroMangaManagerRemote";
goto PackageApks;
// 1. 仅执行 MSIX 打包
MsixPackager msixPackager = new (version , Slnx);
msixPackager.CleanThenRestoreSlnx();
msixPackager.BuildMsix();

Console.WriteLine("MSIX 打包完成，生成文件：");
foreach (var file in msixPackager.Files)
{
    Console.WriteLine(file);
}
GitHubReleasePublisher publisher = new("DJDQfff" , "EroMangaManager");

await publisher.PublishAsync(version , msixPackager.Files);

PackageApks:
// 2. 仅执行 APK 打包
ApkPackager apkPackager = new(version , remoteSlnx);
apkPackager.CleanThenRestoreSlnx();
apkPackager.PublishAPK();

Console.WriteLine("APK 打包完成，生成文件：");
foreach (var file in apkPackager.Files)
{
    Console.WriteLine(file);
}
GitHubReleasePublisher publisher2 = new("DJDQfff" , "EroMangaManagerRemote");

await publisher2.PublishAsync(version , apkPackager.Files);

