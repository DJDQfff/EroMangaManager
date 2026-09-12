using System;
using System.Collections.Generic;
using System.Text;
using Core.Interfaces;
#if WINDOWS
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
#endif
namespace UnoLibrary.Services;

public class Notifier : INotifier
{
    public void Notify(string message, int durationInSeconds = 5)
    {
        if (message == null)
        {
            return;
        }
#if WINDOWS
        var appNotification = new AppNotificationBuilder().AddText(message).BuildNotification();
        AppNotificationManager.Default.Show(appNotification);
#endif
    }

    public void NotifyAccessDenied(string message, int durationInSeconds = 5)
    {
#if WINDOWS
        Notify($"🚫🔒{message}🔒🚫");
#endif
    }

    public void NotifyError(string message, int durationInSeconds = 5)
    {
        Notify($"🚨❌{message}❌🚨");
    }

    public void NotifyFailure(string message, int durationInSeconds = 5)
    {
        Notify($"💥{message}💥");
    }

    public void NotifyWorkDone(string message, int durationInSeconds = 5)
    {
        Notify($"🎉🎉{message}🎉🎉");
    }
}
