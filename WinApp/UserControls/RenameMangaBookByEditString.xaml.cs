namespace EroMangaManager.WinUI3.UserControls;

public sealed partial class RenameMangaByEditString : UserControl
{
    public Manga Manga
    {
        get => DataContext as Manga;
        set
        {
            DataContext = value;
            textbox.Text = value.FileDisplayName;
        }
    }

    private bool isnewnameok;

    /// <summary>
    /// 输入的新名字是否可用
    /// 两个条件：1.新名字不含非法字符。2.新名称与旧名称不相同。
    /// </summary>
    public bool IsNewnameOK
    {
        set { isnewnameok = value; }
        get { return isnewnameok && (NewDisplayName != Manga.FileDisplayName); }
    }

    /// <summary>
    /// 输入不合法
    /// </summary>
    public event Action WrongInput;

    /// <summary>
    /// 输入合法
    /// </summary>
    public event Action CorrectInput;

    /// <summary>
    /// 选择的新名字
    /// </summary>
    public string NewDisplayName => textbox.Text;

    /// <summary>
    ///
    /// </summary>
    public RenameMangaByEditString()
    {
        InitializeComponent(); DataContextChanged += (a, b) => this.Bindings.Update();
    }

    /// <summary>
    /// 不指定新名字，默认去除重复标签
    /// </summary>
    /// <param name="mangaBook"></param>
    public RenameMangaByEditString(Manga mangaBook)
    {
        InitializeComponent();
        Manga = mangaBook;
        //CorrectInput += () => RenameButton.IsEnabled = true;
        //WrongInput += () => RenameButton.IsEnabled = false;
    }

    /// <summary>
    /// 指定新名字
    /// </summary>
    /// <param name="mangaBook"></param>
    /// <param name="suggestedname"></param>
    public RenameMangaByEditString(Manga mangaBook, string suggestedname)
        : base()
    {
        if (suggestedname == null)
        {
            throw new NullReferenceException();
        }
        else
        {
            textbox.Text = suggestedname;
        }
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NewDisplayName))
        {
            // 检查文件名是非为空

            hinttextblock.Text = ResourceLoader
                .GetForViewIndependentUse()
                .GetString("DontUseEmptyString");
            IsNewnameOK = false;
            RenameButton.IsEnabled = false;
            WrongInput?.Invoke();
        }
        else if (NewDisplayName.Any(c => Path.GetInvalidFileNameChars().Contains(c)))
        {
            // 检查文件是否含有非法字符

            hinttextblock.Text = ResourceLoader
                .GetForViewIndependentUse()
                .GetString("ContainInvalaidChar");
            IsNewnameOK = false;
            RenameButton.IsEnabled = false;
            WrongInput?.Invoke();
        }
        else
        {
            // 以上都没问题，那就认为正确
            hinttextblock.Text = string.Empty;
            IsNewnameOK = true;
            RenameButton.IsEnabled = true;
            CorrectInput?.Invoke();
        }
    }

    private async void RenameButton_Click(object sender, RoutedEventArgs e)
    {
        var newname = NewDisplayName;// 由于NewDIsplayName在下面异步访问，会出现ui线程报错，提前取出来
        try
        {
            var manga = Manga;
            string newpath = await Task.Run(() => MangaFileIO.MoveManga(manga, null, newname));
            Manga.FilePath = newpath;
        }
        catch (UnauthorizedAccessException) { App.Current.GlobalViewModel.AccessDenied(); }
        catch (System.IO.IOException) { App.Current.GlobalViewModel.AccessDenied(); }
    }
}