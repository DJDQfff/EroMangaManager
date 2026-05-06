using Config.Net;

using EroMangaManager.Core.Setting;

namespace EroMangaManager.Core.ViewModels
{
    /// <summary>
    /// 设置项ViewModel
    /// </summary>
    /// <remarks>
    ///
    /// </remarks>
    public partial class SettingViewModel : ObservableObject
    {
        /// <summary>
        /// 设置数据源
        /// </summary>
        public IAppConfig AppConfig { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="iniPath"></param>
        public SettingViewModel(string iniPath)
        {
            AppConfig = new ConfigurationBuilder<IAppConfig>().UseIniFile(iniPath).Build();
            var exes = AppConfig.MangaOpenWay3.OpenWays
                    ?.Split('|', '?')

                    .SkipWhile(string.IsNullOrWhiteSpace)
                .Skip(2);
            ExePaths = new(exes);
            DefaultWay = AppConfig.MangaOpenWay3.DefaultWay;
        }

        // TODO 这里疑似有bug。
        // 一开始，winui项目在两个不同的控件直接引用源defalutway（双向绑定），结果两个ui绑定的结果不一致，不能同步。
        // ini文件出现多行defaultway值
        // 然后，在这个viewmodel里面套一层defalutway，那两个控件双向绑定到这个而不是直接到源，ui结果现在保持一致。
        // 但是，两个ui结果还是出现多行defaultway值，疑似config.net库有bug
        [ObservableProperty]
        string defaultWay;
        partial void OnDefaultWayChanged(string value)
        {
            AppConfig.MangaOpenWay3.DefaultWay = value;

        }
        /// <summary>
        /// 存储的exe路径
        /// </summary>
        public ObservableCollection<string> ExePaths { get; }

        /// <summary>
        /// 移除某exe路径
        /// </summary>
        /// <param name="path"></param>
        public void RemoveExePath(string path)
        {
            var str = $"|{path}";
            AppConfig.MangaOpenWay3.OpenWays = AppConfig.MangaOpenWay3.OpenWays.Replace(
                str,
                string.Empty
            );
            _ = ExePaths.Remove(path);
        }

        /// <summary>
        /// 添加exe路径
        /// </summary>
        /// <param name="exePath"></param>
        public void AddExePath(string exePath)
        {
            if (!AppConfig.MangaOpenWay3.OpenWays.Contains(exePath))
            {
                AppConfig.MangaOpenWay3.OpenWays += $"|{exePath}";
                ExePaths.Add(exePath);
            }
        }
    }
}