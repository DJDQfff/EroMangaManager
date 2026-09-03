

using System;
using System.Collections.Generic;
using System.Text;

using UnoLibrary;

using WinApp.Strings;
namespace UnoLibrary;

public interface IPages
{
    NavigationItem Find(StringsEnum uid);
    NavigationItem[] MainPages { get; }
    NavigationItem[] FooterPages { get; }
    NavigationItem SettingPage { get; }
}

