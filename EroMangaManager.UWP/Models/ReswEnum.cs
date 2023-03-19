using Windows.UI.Xaml.Markup;
using Windows.ApplicationModel.Resources;
namespace EroMangaManager.UWP.LocalizationWords
{
    public enum AppxmanifestResourcesReswEnum
    {
        AppDisplayName, Description
    }

    public class AppxmanifestResourcesReswEnumExtension : MarkupExtension
    {
        public AppxmanifestResourcesReswEnum AppxmanifestResourcesReswEnum { get; set; }

        protected override object ProvideValue()
        {
            return ResourceLoader.GetForViewIndependentUse("AppxmanifestResources").GetString(AppxmanifestResourcesReswEnum.ToString());

        }
    }

    public enum ResourcesReswEnum
    {
        AddAFolder, Bookcase, BookcaseItemToolTip, CancleEdit, ChangeSortMethod, Close, CommonSetting, ConfirmEdit, DeleteFile, DeleteIt, DeleteMode, DeleteMode_JustDelete, DeleteMode_MoveTo, DeleteSourceFile, DontShowThisImage, Edit, ErrorZips, ExportToPDF, FilteredImages, FindSameMangaByName, Folders, FullScreen, FunctionOff, FunctionOn, IsEmptyFolderShow, MailFeedbackToDeveloper, MangaTags, MangaTagsCategorys, MangaTagsManage, No, NoFoldersPleaseAddFirst, NoSelectedFolder, NowInitializingMaybeError, NowReading, Open, OpenFile, OpenFolder, OpenInMicrosoftStore, OpenItsPlacedFolder, OverviewInformation, PinnedFolders, Refresh, RemoveFolder, RemoveRepeatTags, RenameFile, SaveAs, Search, SetAsDefaultBookcaseFolder, SetBack, ShowFolderBookcase, Tags, ToolBox, Translate, UpdateRecords, Usage, WhetherOpenFilterImageFunction, WhetherShowDialogBeforeDelete, Yes
    }

    public class ResourcesReswEnumExtension : MarkupExtension
    {
        public ResourcesReswEnum ResourcesReswEnum { get; set; }

        protected override object ProvideValue()
        {
            return ResourceLoader.GetForViewIndependentUse("Resources").GetString(ResourcesReswEnum.ToString());

        }
    }

    public enum StringResourcesReswEnum
    {
        ContainInvalaidChar, DontUseEmptyString, ErrorString1, ExportDone
    }

    public class StringResourcesReswEnumExtension : MarkupExtension
    {
        public StringResourcesReswEnum StringResourcesReswEnum { get; set; }

        protected override object ProvideValue()
        {
            return ResourceLoader.GetForViewIndependentUse("StringResources").GetString(StringResourcesReswEnum.ToString());

        }
    }

    public enum UpdateResourcesReswEnum
    {
        Description_20221219, Description_20221226, Description_20230109, Description_20230125, Description_20230205, Description_before, Version_20221219, Version_20221226, Version_20230109, Version_20230125, Version_20230205, Version_before
    }

    public class UpdateResourcesReswEnumExtension : MarkupExtension
    {
        public UpdateResourcesReswEnum UpdateResourcesReswEnum { get; set; }

        protected override object ProvideValue()
        {
            return ResourceLoader.GetForViewIndependentUse("UpdateResources").GetString(UpdateResourcesReswEnum.ToString());

        }
    }

    public enum UsageReswEnum
    {
        SupportedTypeContent, SupportedTypeTitle, WhatISTheAppContent, WhatISTheAppTitle, WhyZipRatherFolderContent, WhyZipRatherFolderTitle
    }

    public class UsageReswEnumExtension : MarkupExtension
    {
        public UsageReswEnum UsageReswEnum { get; set; }

        protected override object ProvideValue()
        {
            return ResourceLoader.GetForViewIndependentUse("Usage").GetString(UsageReswEnum.ToString());

        }
    }

}
