using System.Runtime.InteropServices;

namespace Core;

public partial class ExplorerFile
{
    // 1. ILFree 方法：仅涉及 IntPtr，无字符串，直接替换即可
    [LibraryImport("shell32.dll")]
    private static partial void ILFree(IntPtr pidlList);

    // 2. ILCreateFromPathW 方法：包含 string 参数，必须显式指定 UTF-16 编码
    [LibraryImport("shell32.dll", StringMarshalling = StringMarshalling.Utf16)]
    private static partial IntPtr ILCreateFromPathW(string pszPath);

    // 3. SHOpenFolderAndSelectItems 方法：仅涉及 IntPtr 和基础值类型，直接替换即可
    [LibraryImport("shell32.dll")]
    private static partial int SHOpenFolderAndSelectItems(
        IntPtr pidlList,
        uint cild,
        IntPtr children,
        uint dwFlags
    );

    /// <summary>
    /// 调用系统资源浏览器选中文件或文件夹。
    /// 文件调用shell打开，文件夹调用Process打开
    /// </summary>
    /// <param name="filePath"></param>
    public static void ExplorerSelectFile(string filePath)
    {
        if (!File.Exists(filePath) && !Directory.Exists(filePath))
            return;

        if (Directory.Exists(filePath))
            Process.Start(@"explorer.exe", "/select,\"" + filePath + "\"");
        else
        {
            IntPtr pidlList = ILCreateFromPathW(filePath);
            if (pidlList != IntPtr.Zero)
            {
                try
                {
                    Marshal.ThrowExceptionForHR(
                        SHOpenFolderAndSelectItems(pidlList, 0, IntPtr.Zero, 0)
                    );
                }
                finally
                {
                    ILFree(pidlList);
                }
            }
        }
    }
    #region 旧语法
    //[DllImport("shell32.dll", ExactSpelling = true)]
    //private static extern void ILFree(IntPtr pidlList);

    //[DllImport("shell32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    //private static extern IntPtr ILCreateFromPathW(string pszPath);

    //[DllImport("shell32.dll", ExactSpelling = true)]
    //private static extern int SHOpenFolderAndSelectItems(
    //    IntPtr pidlList,
    //    uint cild,
    //    IntPtr children,
    //    uint dwFlags
    //);
    #endregion
}
