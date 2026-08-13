using Windows.ApplicationModel.DataTransfer;

namespace WinApp.Services;

public class ClipboardHelper
{
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
