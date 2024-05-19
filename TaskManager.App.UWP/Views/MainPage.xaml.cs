using System;

using TaskManager.App.UWP.ViewModels;

using Windows.UI.Xaml.Controls;

namespace TaskManager.App.UWP.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();
        }
    }
}
