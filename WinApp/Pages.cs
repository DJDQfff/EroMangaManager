using System;
using System.Collections.Generic;
using System.Text;

using WinApp.Strings;

namespace WinApp;

public class Pages
{
    public NavigationItem Find(StringsEnum uid)
    {
        var item = MainPages.Concat(FooterPages).Concat([SettingPage]).SingleOrDefault(x => x.Uid == uid);
        if (item is null)
        {
            throw new Exception($"未找到页面 {uid}");
        }
        return item;
    }
   public NavigationItem[] MainPages = [
    
        new(typeof(Bookcase),StringsEnum.Bookcase,StringsExtension.ResourceLoader.GetString(StringsEnum.Bookcase.ToString()) , new(Symbol.ViewAll)),
        new(typeof(LibraryPage),StringsEnum.Library, StringsExtension.ResourceLoader.GetString(StringsEnum.Library.ToString()), new(Symbol.Library)),
        new(typeof(GlobalSearchPage),StringsEnum.GlobalSearch, StringsExtension.ResourceLoader.GetString(StringsEnum.GlobalSearch.ToString()), new(Symbol.Find)),
        new(typeof(TagsManagePage),StringsEnum.MangaTagsManage, StringsExtension.ResourceLoader.GetString(StringsEnum.MangaTagsManage.ToString()), new(Symbol.Manage)),
        new(typeof(FindSameManga),StringsEnum.FindSameMangaByName, StringsExtension.ResourceLoader.GetString(StringsEnum.FindSameMangaByName.ToString()), new(Symbol.Copy)),
        //new(typeof(RemoveRepeatTags2), StringsExtension.ResourceLoader.GetString(StringsEnum.RemoveRepeatTags.ToString()), new(Symbol.Tag)),
        new(typeof(IrregularNameSearch),StringsEnum.IrregularName, StringsExtension.ResourceLoader.GetString(StringsEnum.IrregularName.ToString()), new(Symbol.Edit)),
        new(typeof(ServerPage),StringsEnum.Server, StringsExtension.ResourceLoader.GetString(StringsEnum.Server.ToString()), new(Symbol.Remote)),
    ];

    public NavigationItem[] FooterPages =
    [
                     new(typeof(UsageDocumentPage),StringsEnum.Usage, StringsExtension.ResourceLoader.GetString(StringsEnum.Usage.ToString()), new(Symbol.Help)),
            //new(typeof(UpdateRecordsPage), StringsExtension.ResourceLoader.GetString(StringsEnum.UpdateRecords.ToString()), new(Symbol.ShowResults) ),

    ];


    public NavigationItem SettingPage = new (typeof(SettingPage),StringsEnum.Setting , StringsExtension.ResourceLoader.GetString(StringsEnum.Setting.ToString()) , new(Symbol.Setting));
}
