using Windows.UI.Xaml.Markup;
using Windows.ApplicationModel.Resources;
namespace EroMangaManager.UWP.LocalizationWords
{
    ///<summary> </summary>
    public enum AppxmanifestResources
    {
        ///<summary></summary> 
        AppDisplayName,
        ///<summary></summary> 
        Description,

    }
    ///<summary> </summary>
    public class AppxmanifestResourcesExtension : MarkupExtension
    {
        ///<summary> </summary>
        public AppxmanifestResources AppxmanifestResources { get; set; }

        ///<summary> </summary>
        protected override object ProvideValue()
        {
            return ResourceLoader.GetForViewIndependentUse("AppxmanifestResources").GetString(AppxmanifestResources.ToString());

        }
    }

    ///<summary> </summary>
    public enum Resources
    {
        ///<summary></summary> 
        AddAFolder,
        ///<summary></summary> 
        Bookcase,
        ///<summary></summary> 
        BookcaseItemToolTip,
        ///<summary></summary> 
        CancleEdit,
        ///<summary></summary> 
        ChangeSortMethod,
        ///<summary></summary> 
        Close,
        ///<summary></summary> 
        CommonSetting,
        ///<summary></summary> 
        ConfirmEdit,
        ///<summary></summary> 
        DeleteFile,
        ///<summary></summary> 
        DeleteIt,
        ///<summary></summary> 
        DeleteMode,
        ///<summary></summary> 
        DeleteMode_JustDelete,
        ///<summary></summary> 
        DeleteMode_MoveTo,
        ///<summary></summary> 
        DeleteSourceFile,
        ///<summary></summary> 
        DontShowThisImage,
        ///<summary></summary> 
        Edit,
        ///<summary></summary> 
        ErrorZips,
        ///<summary></summary> 
        ExportToPDF,
        ///<summary></summary> 
        FilteredImages,
        ///<summary></summary> 
        FindSameMangaByName,
        ///<summary></summary> 
        Folders,
        ///<summary></summary> 
        FullScreen,
        ///<summary></summary> 
        FunctionOff,
        ///<summary></summary> 
        FunctionOn,
        ///<summary></summary> 
        GitHubFeedBack,
        ///<summary></summary> 
        IsEmptyFolderShow,
        ///<summary></summary> 
        MailFeedbackToDeveloper,
        ///<summary></summary> 
        MangaTags,
        ///<summary></summary> 
        MangaTagsCategorys,
        ///<summary></summary> 
        MangaTagsManage,
        ///<summary></summary> 
        No,
        ///<summary></summary> 
        NoFoldersPleaseAddFirst,
        ///<summary></summary> 
        NoSelectedFolder,
        ///<summary></summary> 
        NowInitializingMaybeError,
        ///<summary></summary> 
        NowReading,
        ///<summary></summary> 
        Open,
        ///<summary></summary> 
        OpenFile,
        ///<summary></summary> 
        OpenFolder,
        ///<summary></summary> 
        OpenInMicrosoftStore,
        ///<summary></summary> 
        OpenItsPlacedFolder,
        ///<summary></summary> 
        OverviewInformation,
        ///<summary></summary> 
        PinnedFolders,
        ///<summary></summary> 
        Refresh,
        ///<summary></summary> 
        RemoveFolder,
        ///<summary></summary> 
        RemoveRepeatTags,
        ///<summary></summary> 
        RenameFile,
        ///<summary></summary> 
        SaveAs,
        ///<summary></summary> 
        Search,
        ///<summary></summary> 
        SetAsDefaultBookcaseFolder,
        ///<summary></summary> 
        SetBack,
        ///<summary></summary> 
        ShowFolderBookcase,
        ///<summary></summary> 
        Tags,
        ///<summary></summary> 
        ToolBox,
        ///<summary></summary> 
        Translate,
        ///<summary></summary> 
        UpdateRecords,
        ///<summary></summary> 
        Usage,
        ///<summary></summary> 
        WhetherOpenFilterImageFunction,
        ///<summary></summary> 
        WhetherShowDialogBeforeDelete,
        ///<summary></summary> 
        Yes,

    }
    ///<summary> </summary>
    public class ResourcesExtension : MarkupExtension
    {
        ///<summary> </summary>
        public Resources Resources { get; set; }

        ///<summary> </summary>
        protected override object ProvideValue()
        {
            return ResourceLoader.GetForViewIndependentUse("Resources").GetString(Resources.ToString());

        }
    }

    ///<summary> </summary>
    public enum StringResources
    {
        ///<summary></summary> 
        ContainInvalaidChar,
        ///<summary></summary> 
        DontUseEmptyString,
        ///<summary></summary> 
        ErrorString1,
        ///<summary></summary> 
        ExportDone,

    }
    ///<summary> </summary>
    public class StringResourcesExtension : MarkupExtension
    {
        ///<summary> </summary>
        public StringResources StringResources { get; set; }

        ///<summary> </summary>
        protected override object ProvideValue()
        {
            return ResourceLoader.GetForViewIndependentUse("StringResources").GetString(StringResources.ToString());

        }
    }

    ///<summary> </summary>
    public enum UpdateResources
    {
        ///<summary></summary> 
        Description_20221219,
        ///<summary></summary> 
        Description_20221226,
        ///<summary></summary> 
        Description_20230109,
        ///<summary></summary> 
        Description_20230125,
        ///<summary></summary> 
        Description_20230205,
        ///<summary></summary> 
        Description_before,
        ///<summary></summary> 
        Version_20221219,
        ///<summary></summary> 
        Version_20221226,
        ///<summary></summary> 
        Version_20230109,
        ///<summary></summary> 
        Version_20230125,
        ///<summary></summary> 
        Version_20230205,
        ///<summary></summary> 
        Version_before,

    }
    ///<summary> </summary>
    public class UpdateResourcesExtension : MarkupExtension
    {
        ///<summary> </summary>
        public UpdateResources UpdateResources { get; set; }

        ///<summary> </summary>
        protected override object ProvideValue()
        {
            return ResourceLoader.GetForViewIndependentUse("UpdateResources").GetString(UpdateResources.ToString());

        }
    }

    ///<summary> </summary>
    public enum Usage
    {
        ///<summary></summary> 
        SupportedTypeContent,
        ///<summary></summary> 
        SupportedTypeTitle,
        ///<summary></summary> 
        WhatISTheAppContent,
        ///<summary></summary> 
        WhatISTheAppTitle,
        ///<summary></summary> 
        WhyZipRatherFolderContent,
        ///<summary></summary> 
        WhyZipRatherFolderTitle,

    }
    ///<summary> </summary>
    public class UsageExtension : MarkupExtension
    {
        ///<summary> </summary>
        public Usage Usage { get; set; }

        ///<summary> </summary>
        protected override object ProvideValue()
        {
            return ResourceLoader.GetForViewIndependentUse("Usage").GetString(Usage.ToString());

        }
    }

}
