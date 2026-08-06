namespace Core.Setting;

/// <summary>
/// 设置文件路径接口
/// </summary>
public interface ISettingFilePath
{
    /// <summary>
    /// 获取设置文件路径
    /// </summary>
    public string IniPath { get; }
}
