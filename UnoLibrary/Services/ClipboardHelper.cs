using CommunityToolkit.Mvvm.Input;
using Windows.ApplicationModel.DataTransfer;

namespace UnoLibrary.Services;

public partial class ClipboardHelper
{
    [RelayCommand]
    public void Copy(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;

        DataPackage dataPackage = new();
        dataPackage.SetText(text);
        dataPackage.RequestedOperation = DataPackageOperation.Copy;
        Clipboard.SetContent(dataPackage);
    }
}
