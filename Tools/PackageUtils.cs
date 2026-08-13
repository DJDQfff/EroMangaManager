using System.Diagnostics;

namespace Tools;

/// <summary>
/// 打包过程使用的公共工具类
/// </summary>
public static class PackagingUtils
{
    public const string PfxFile = @"E:\Projects\DJDQfff_new.pfx";
    /// <summary>
    /// 获取环境变量中的证书密码
    /// </summary>
    public static string GetPfxPassword ()
    {
        return Environment.GetEnvironmentVariable("MADAO_PASSWORD")
               ?? throw new ArgumentException("环境变量：证书密码未配置");
    }

    /// <summary>
    /// 执行外部命令并等待退出
    /// </summary>
    public static void Run (string fileName , string arguments , string workingDir)
    {
        var psi = new ProcessStartInfo
        {
            FileName = fileName ,
            Arguments = arguments ,
            WorkingDirectory = workingDir ,
            UseShellExecute = false ,
            CreateNoWindow = false ,
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