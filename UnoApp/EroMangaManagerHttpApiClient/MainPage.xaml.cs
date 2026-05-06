
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using EroMangaManager.Core.DTOs;
using Microsoft.UI.Xaml.Media.Imaging;
using Uno.UI.Extensions;
using Windows.System;

namespace EroMangaManagerHttpApiClient;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
    }
    override protected async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (await App.MangaClient.CheckConnectionAsync() )
        {            mainFrame.Navigate(typeof(NavigationPage));

        }
        else
        {            mainFrame.Navigate(typeof(ConnectPage));

        }
    }

}
