using System;
using System.Collections.Generic;
using System.Text;

namespace EroMangaManagerHttpApiClient
{
    class AndroidOperation
    {
        public static void Open( StorageFile storagefile)
        {
#if ANDROID
        Java.IO.File filelocation = new Java.IO.File(storagefile.Path);

        // 1. 将文件路径转换为 Uri
        // 注意：Android 7.0+ 需要使用 FileProvider，这里为了演示简单使用 fromFile
        // 生产环境建议配置 FileProvider 以兼容 Android 7.0+
        //Android.Net.Uri uri= Android.Net.Uri.FromFile(filelocation);

        // 2. 获取 Content URI (核心修改)
        // 参数说明：
        // - context: 当前上下文
        // - authority: 必须与 AndroidManifest.xml 中配置的 authorities 完全一致
        // - file: 文件对象
        string authority = Android.App.Application.Context.PackageName + ".fileprovider";
        Android.Net.Uri uri = AndroidX.Core.Content.FileProvider.GetUriForFile(
            Android.App.Application.Context,
            authority,
            filelocation
        );
        // 2. 创建 Intent，动作为 ACTION_VIEW (查看)
        var intent = new Android.Content.Intent(Android.Content.Intent.ActionView);
        // 3. 设置数据和类型 (MIME 类型很重要，比如 "application/pdf")
        intent.SetDataAndType(uri, "application/zip");
        // 4. 添加标志位
        intent.AddFlags( Android.Content.ActivityFlags.GrantReadUriPermission); // 授权读取权限
        intent.AddFlags(Android.Content.ActivityFlags.NewTask);       // 在新任务栈启动

        // 5. 启动
        Android.App.Application.Context.StartActivity(intent);
#endif

        }
    }
}
