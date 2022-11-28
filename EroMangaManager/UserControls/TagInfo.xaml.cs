using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;

using EroMangaDB.Entities;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using EroMangaManager.Models;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace EroMangaManager.UserControls
{
    /// <summary>
    /// 一堆Tag
    /// </summary>
    public sealed partial class TagInfo : UserControl
    {
        /// <summary>
        /// 标签名
        /// </summary>
        public string TagName { get; set; }
        /// <summary>
        /// 标签值
        /// </summary>
        public string TagValue { get; set; }

        /// <summary>
        /// 构造
        /// </summary>
        public TagInfo ()
        {
            this.InitializeComponent();
        }
    }
}