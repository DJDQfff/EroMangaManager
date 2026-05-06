using EroMangaManager.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EroMangaManager.WinUI3.Views.MainPageChildPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ServerPage : Page
    {
        ASPNETCoreServer server;
        public ServerPage()
        {
            InitializeComponent();

               server = new ASPNETCoreServer(App.Current.GlobalViewModel);

            

            server.EventDeleteMang += StorageOperation.Delete;

            server.EventDeleteMang += (Manga manga) =>
            {
                this.DispatcherQueue.TryEnqueue(() =>
                {                _ =  App.Current.GlobalViewModel.RemoveManga(manga); 

                });
            };
           
            this.DataContext = server;

        }


        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
           await  server.StartServer();
        }
    }
}
