using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interfaces;

/// <summary>
/// 通知器
/// </summary>
public interface INotifier
{
    /// <summary>
    /// 通用
    /// </summary>
    /// <param name="message"></param>
    /// <param name="durationInSeconds"></param>
    void Notify(string message, int durationInSeconds = 5);

    /// <summary>
    /// 错误
    /// </summary>
    /// <param name="message"></param>
    /// <param name="durationInSeconds"></param>
    void NotifyError(string message, int durationInSeconds = 5);

    /// <summary>
    /// 任务失败
    /// </summary>
    /// <param name="message"></param>
    /// <param name="durationInSeconds"></param>
    void NotifyFailure(string message, int durationInSeconds = 5);

    /// <summary>
    /// 访问被拒绝
    /// </summary>
    /// <param name="message"></param>
    /// <param name="durationInSeconds"></param>
    void NotifyAccessDenied(string message, int durationInSeconds = 5);

    /// <summary>
    /// 任务完成
    /// </summary>
    /// <param name="message"></param>
    /// <param name="durationInSeconds"></param>
    void NotifyWorkDone(string message, int durationInSeconds = 5);

    /// <summary>
    /// 错误图标
    /// </summary>
    public static class ErrorSymbols
    {
        /// <summary>
        /// 叉
        /// </summary>
        public const string GeneralFailure = "\u274C"; // ❌

        /// <summary>
        /// 感叹号
        /// </summary>
        public const string Warning = "\u26A0\uFE0F"; // ⚠️ (注意变体选择器)

        /// <summary>
        /// 拒绝
        /// </summary>
        public const string AccessDenied = "\U0001F6AB"; // 🚫

        /// <summary>
        /// 拒绝
        /// </summary>
        public const string Blocked = "\u26D4"; // ⛔

        /// <summary>
        /// 爆炸
        /// </summary>
        public const string Crash = "\U0001F4A5"; // 💥

        /// <summary>
        /// 计时
        /// </summary>
        public const string Timeout = "\u23F3"; // ⏳
    }
}
