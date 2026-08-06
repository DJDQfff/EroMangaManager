// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinApp.UserControls;

public sealed partial class TagListOrder : UserControl, INotifyPropertyChanged
{
    public IEnumerable<string> Sources
    {
        set
        {
            field = value;
            ListView1.Items.Clear();
            ListView2.Items.Clear();
            foreach (var ss in value)
            {
                ListView1.Items.Add(ss);
            }
        }
        get;
    } = [];

    public event PropertyChangedEventHandler? PropertyChanged;

    public string NewName
    {
        get
        {
            List<string> list = [];
            foreach (var tag in ListView1.Items)
            {
                if (tag is string str)
                {
                    list.Add(str);
                }
            }
            return string.Concat(list);
        }
    }

    private void InvokeNewName()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewName)));
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            var tag = button.DataContext as string;
            var container = ListView2.ContainerFromItem(tag);
            var index = ListView2.IndexFromContainer(container);

            ListView2.Items.RemoveAt(index);
            ListView1.Items.Add(tag);
        }
        //SetNewName();
        InvokeNewName();
    }

    private void RemoveButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            var tag = button.DataContext as string;
            var container = ListView1.ContainerFromItem(tag);
            var index = ListView1.IndexFromContainer(container);

            ListView1.Items.RemoveAt(index);
            ListView2.Items.Add(tag);
        }
        //SetNewName();
        InvokeNewName();
    }

    //public void SetNewName()
    //{
    //    var list = new List<string>();
    //    foreach (var tag in ListView1.Items)
    //    {
    //        list.Add(tag as string);
    //    }
    //    NewName = string.Concat( list);
    //}

    public TagListOrder()
    {
        InitializeComponent();
    }
}
