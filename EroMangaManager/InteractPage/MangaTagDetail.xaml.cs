﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using EroMangaManager.Models;
using EroMangaManager.UserControls;
using EroMangaDB.Entities;
using EroMangaDB;
using EroMangaDB.EntityFactory;
using static EroMangaDB.BasicController;

// https://go.microsoft.com/fwlink/?LinkId=234238
// 上介绍了“内容对话框”项模板

namespace EroMangaManager.InteractPage
{
    public sealed partial class MangaTagDetail : ContentDialog
    {
        private ReadingInfo readingInfo;

        public MangaTagDetail (MangaBook _mangaBook)
        {
            this.InitializeComponent();
            readingInfo = _mangaBook.ReadingInfo;
            var tags = readingInfo.TagPieces.Split('\r');
            var list = DatabaseController.MatchTag(tags);
            foreach (var l in list)
            {
                TagInfo tag = new TagInfo()
                {
                    TagName = l.Value ?? "未知标签",
                    TagValue = l.Key
                };
                Stack.Children.Add(tag);
            }
        }

        private string ConvertTagValue (object tagvalue)
        {
            switch (tagvalue)
            {
                case true:
                    return "是";

                case false:
                    return "否";

                case "English":
                    return "英语";

                case "Chinese":
                    return "中文";

                case "Japanese":
                    return "日语";

                case string str:
                    return str;

                default:
                    return "未知";
            }
        }
    }
}