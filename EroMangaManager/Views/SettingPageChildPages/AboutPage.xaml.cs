﻿using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace EroMangaManager.Views.SettingPageChildPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        /// <summary>
        ///
        /// </summary>
        public AboutPage ()
        {
            this.InitializeComponent();
        }

        private void HyperlinkButton_Click (object sender , Windows.UI.Xaml.RoutedEventArgs e)
        {
            var mail=new System.Uri("mailto:2743109998@qq.com?subject=jflaj?body=djflkajdkfjaj");
        }
    }
}