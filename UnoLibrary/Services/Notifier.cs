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
#if WINDOWS
        var appNotification = new AppNotificationBuilder().AddText(message).BuildNotification();
        AppNotificationManager.Default.Show(appNotification);
#endif
    }
}
