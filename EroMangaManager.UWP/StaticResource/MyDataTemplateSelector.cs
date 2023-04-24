using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpCompress.Archives;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml;

namespace EroMangaManager.UWP.StaticResource
{
    public class MyDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate OfBitmapImage { get; set; }
        public DataTemplate OfIArchiveEntry { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            switch (item)
            {
                case BitmapImage a:
                    return OfIArchiveEntry;

                case IArchiveEntry s:
                    return OfIArchiveEntry;

                case var s:
                    return OfBitmapImage;
                    break;
            }
            return null;
        }
    }
}