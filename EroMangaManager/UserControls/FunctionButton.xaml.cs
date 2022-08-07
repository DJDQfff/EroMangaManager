using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace EroMangaManager.UserControls
{
    public sealed partial class FunctionButton : UserControl
    {
        public FunctionButton()
        {
            this.InitializeComponent();

            Type type = typeof(Windows.UI.Colors);
            var a = type.GetProperties();
            Random random = new Random();
            var b = random.Next(0, a.Length);
            var c = a[b];
            var d = (Windows.UI.Color)c.GetValue(b);

            SolidColorBrush solidColorBrush = new SolidColorBrush(d);

            this.ControlButton.Background = solidColorBrush;
        }
    }
}