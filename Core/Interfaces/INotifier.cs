using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interfaces;

public interface INotifier
{
    void Notify(string message, int durationInSeconds = 5);

    public static class ErrorSymbols
    {
        public const string GeneralFailure = "\u274C"; // ❌
        public const string Warning = "\u26A0\uFE0F"; // ⚠️ (注意变体选择器)
        public const string AccessDenied = "\U0001F6AB"; // 🚫
        public const string Blocked = "\u26D4"; // ⛔
        public const string Crash = "\U0001F4A5"; // 💥
        public const string Timeout = "\u23F3"; // ⏳
    }
}
