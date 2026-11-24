
using System.Runtime.InteropServices;

namespace Core;

/// <summary>
/// 调用平台外部程序：系统默认程序打开、指定程序打开、在文件管理器中定位。
/// 三个方法共享"调用外部程序"这一职责，平台分支各自封装在私有方法内。
/// </summary>
public partial class FileLauncher
{
    // ==================== 公开入口 ====================

    /// <summary>用系统默认关联程序打开文件。</summary>
    public static void OpenWithSystemDefaultApp(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("文件路径不能为空。", nameof(filePath));

        if (OperatingSystem.IsWindows())
            OpenDefaultOnWindows(filePath);
        else if (OperatingSystem.IsLinux())
            OpenDefaultOnLinux(filePath);
        else
            throw new PlatformNotSupportedException("当前平台不支持默认程序打开。");
    }

    /// <summary>用用户指定的外部程序打开文件（Windows/Linux 通用，无分支）。</summary>
    public static Process? OpenWithSpecifiedAppAndReturn(string appPath, string filePath)
    {
        if (string.IsNullOrWhiteSpace(appPath))
            throw new ArgumentException("程序路径不能为空。", nameof(appPath));
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("文件路径不能为空。", nameof(filePath));

        var psi = new ProcessStartInfo
        {
            FileName = appPath,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        psi.ArgumentList.Add(filePath);
       return Process.Start(psi);
    }

    /// <summary>在系统文件管理器中定位并选中文件/文件夹。</summary>
    public static void LocateInFileManager(string filePath)
    {
        if (!File.Exists(filePath) && !Directory.Exists(filePath))
            return;

        if (OperatingSystem.IsWindows())
            LocateOnWindows(filePath);
        else if (OperatingSystem.IsLinux())
            LocateOnLinux(filePath);
        // 其他平台可按需扩展
    }

    // ==================== 需求一：默认程序 ====================

    private static void OpenDefaultOnWindows(string filePath)
    {
        Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
    }

    private static void OpenDefaultOnLinux(string filePath)
    {
        var psi = new ProcessStartInfo("xdg-open")
        {
            UseShellExecute = false,
            CreateNoWindow = true
        };
        psi.ArgumentList.Add(filePath);
        Process.Start(psi);
    }

    // ==================== 需求三：文件管理器定位 ====================

    private static void LocateOnWindows(string filePath)
    {  // 文件夹：打开父目录并选中它（或用 explorer 直接打开，看你想要哪种语义）
        if (Directory.Exists(filePath))
        {
            Process.Start("explorer.exe", $"/select,\"{filePath}\"");
            return;
        }
        // 文件：用 Shell32 打开父目录并选中文件
        IntPtr pidlList = ILCreateFromPathW(filePath);
        if (pidlList == IntPtr.Zero) return;

        try
        {
            Marshal.ThrowExceptionForHR(
                SHOpenFolderAndSelectItems(pidlList, 0, IntPtr.Zero, 0));
        }
        finally
        {
            ILFree(pidlList);
        }
    }

    private static void LocateOnLinux(string filePath)
    {
        if (Directory.Exists(filePath))
        {
            Process.Start("xdg-open", filePath);
            return;
        }

        if (TryShowViaDbus(filePath))
            return;

        string? dir = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(dir))
            Process.Start("xdg-open", dir);
    }

    private static bool TryShowViaDbus(string filePath)
    {
        try
        {
            string uri = new Uri(filePath).AbsoluteUri;
            var psi = new ProcessStartInfo("dbus-send")
            {
                UseShellExecute = false,
                RedirectStandardError = true
            };
            foreach (var arg in new[]
            {
                "--session", "--print-reply",
                "--dest=org.freedesktop.FileManager1",
                "/org/freedesktop/FileManager1",
                "org.freedesktop.FileManager1.ShowItems",
                $"array:string:{uri}", "string:"
            })
                psi.ArgumentList.Add(arg);

            using var proc = Process.Start(psi);
            return proc != null && proc.WaitForExit(3000) && proc.ExitCode == 0;
        }
        catch { return false; }
    }

    // ==================== Windows P/Invoke ====================

    [LibraryImport("shell32.dll")]
    private static partial void ILFree(IntPtr pidlList);

    [LibraryImport("shell32.dll", StringMarshalling = StringMarshalling.Utf16)]
    private static partial IntPtr ILCreateFromPathW(string pszPath);

    [LibraryImport("shell32.dll")]
    private static partial int SHOpenFolderAndSelectItems(
        IntPtr pidlList, uint cild, IntPtr children, uint dwFlags);
}