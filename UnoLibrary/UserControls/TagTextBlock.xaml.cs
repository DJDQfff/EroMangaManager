// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using System.Windows.Input;

namespace WinApp.UserControls.MangaBasicInfo;

public sealed partial class TagTextBlock : UserControl
{
    public static readonly DependencyProperty CopyCommandProperty = DependencyProperty.Register(
        nameof(CopyCommand),
        typeof(ICommand),
        typeof(TagTextBlock), // ← 注意：这里填你的控件类名
        new PropertyMetadata(null)
    );

    public ICommand CopyCommand
    {
        get => (ICommand)GetValue(CopyCommandProperty);
        set => SetValue(CopyCommandProperty, value);
    }
    public static readonly DependencyProperty NavigateCommandProperty = DependencyProperty.Register(
        nameof(NavigateCommand),
        typeof(ICommand),
        typeof(TagTextBlock), // ← 注意：这里填你的控件类名
        new PropertyMetadata(null)
    );

    public ICommand NavigateCommand
    {
        get => (ICommand)GetValue(NavigateCommandProperty);
        set => SetValue(NavigateCommandProperty, value);
    }

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text),
        typeof(string),
        typeof(TagTextBlock),
        new PropertyMetadata(null)
    );

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public TagTextBlock()
    {
        InitializeComponent();
    }
}
