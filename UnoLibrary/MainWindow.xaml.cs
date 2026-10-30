using Microsoft.UI.Xaml.Shapes;

namespace UnoLibrary;

/// <summary>
/// WinApp 与 UnoApp 共用的主界面外壳：顶部自定义标题栏占位 + 页面容器。
/// <para>
/// WinApp 把它放进自建的 <c>Window</c>（并用 <see cref="TitleBarElement"/> 设置自定义标题栏），
/// UnoApp 把它放进平台自带的窗口（隐藏标题栏，外面再套 SafeArea）。
/// 这里不持有任何页面字段、也不做导航，页面由调用方（App）通过 <see cref="SetPage{TPage}"/> 指定。
/// </para>
/// </summary>
public sealed partial class MainWindow : UserControl
{
    public MainWindow()
    {
        InitializeComponent();

        // 安全区处理（现在只对 Uno 的移动目标生效）：
        //  - UNO_SINGLE_WINDOW 由 UnoLibrary.csproj 只在 android/ios/browserwasm 目标定义，
        //    所以 windows/desktop 目标下这段是"非活动代码"（VS 里会显示成灰色，属正常，并没有被删）；
        //  - 迁移 WinApp 到 Uno Platform 之后，请删除这段代码，改为在 MainWindow.xaml 里直接用
        //    <utu:SafeArea Insets="Top"> 包住 Grid（具体步骤见 MainWindow.xaml 顶部注释），
        //    并把 UnoLibrary.csproj 条件组里的 Toolkit 移进全局 UnoFeatures。
#if UNO_SINGLE_WINDOW
        // 移动端：默认就把内容放进 SafeArea，避免状态栏/刘海遮挡（原来这层在 UnoApp 的 XAML 里）
        var safeArea = new Uno.Toolkit.UI.SafeArea { Content = RootGrid };
        Uno.Toolkit.UI.SafeArea.SetInsets(safeArea, Uno.Toolkit.UI.SafeArea.InsetMask.Top);
        Content = safeArea;
#endif
    }

    /// <summary> 供 WinApp 调用 <c>Window.SetTitleBar</c> 的可拖拽区域 </summary>
    public Rectangle TitleBarElement => newtitlebar;

    /// <summary>
    /// 是否显示顶部自定义标题栏占位。
    /// WinApp 需要（自定义标题栏），移动端不需要，设为 false 可把整行高度收成 0。
    /// </summary>
    public bool IsTitleBarVisible
    {
        get => TitleBarRow.Height.Value > 0;
        set
        {
            TitleBarRow.Height = new GridLength(value ? 36 : 0);
            newtitlebar.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    /// <summary>
    /// 把页面放进内容容器。TPage 由调用方决定（WinApp 用 MainPage，UnoApp 也用 MainPage，
    /// 需要直接显示别的页面时传 NavigationPage 之类的即可）。
    /// </summary>
    public void SetPage<TPage>(TPage page)
        where TPage : Page
    {
        // 先清空，避免同一个页面实例被同时挂在两处
        PageContainer.Content = null;
        PageContainer.Content = page;
    }
}
