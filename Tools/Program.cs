global using static System.Console;
using Tools;

var version = "2026.8.7";
var slnfolder = "E:\\Projects\\EroMangaManager";

GitHubReleasePublisher publisher = new("DJDQfff", "EroMangaManager");

DotnetMakePackages maker = new(version, slnfolder);

//maker. CleanThenRestoreSlnx();
maker.PubllishAPK();
maker.BuildMsix();

await publisher.PublishAsync(version, maker.Files);
