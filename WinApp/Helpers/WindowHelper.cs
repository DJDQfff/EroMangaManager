namespace EroMangaManager.WinUI3.Helpers
{
    internal static class WindowHelper
    {
        internal static void ShowReadWindow(Manga mangaBook)
        {
            var newWindow = new MainWindow();
            switch (mangaBook.Type)
            {
                case "":

                    {
                        newWindow.MainWindowFrame.Navigate(typeof(ReadPage), mangaBook);

                        //newWindow.MainWindowFrame.Navigate(typeof(ReadPage2) , mangaBook);
                    }
                    break;
                default:

                    {
                        newWindow.MainWindowFrame.Navigate(typeof(ReadPage), mangaBook);
                    }
                    break;
            }
            newWindow.Title = mangaBook.FileDisplayName;

            newWindow.Activate();
        }
    }
}
