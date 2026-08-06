// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinApp.UserControls;

public sealed partial class TagListOrder2 : UserControl
{
    /// <summary>
    /// ������ק��������
    /// </summary>
    public TagListOrder2()
    {
        InitializeComponent();
    }

    public string NewName
    {
        get
        {
            //var items = MangaListView.Items;
            List<string> strings = [];
            //foreach (var item in items)
            //{
            //    var container = MangaListView.ContainerFromItem(item);
            //    var checkbox = container.
            //    var text = container.Content as string;
            //    strings.Add(text);
            //}
            return string.Concat(strings);
        }
    }

    public IEnumerable<string> Sources
    {
        set
        {
            grdiview.Items.Clear();
            foreach (var item in value)
            {
                grdiview.Items.Add(item);
            }
        }
    }
}
