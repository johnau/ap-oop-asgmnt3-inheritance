using Microsoft.Extensions.DependencyInjection;
using System;

using TaskManager.App.UWP.ViewModels;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace TaskManager.App.UWP.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; }

        public MainPage()
        {
            InitializeComponent();

            /**
             * Dependency Injection Note:
             * UWP Apps don't allow constructors with parameters, so we must manually 
             * get the dependency.  
             * There is a workaround by a custom implementation of 
             * IXamlType.ActivateInstance() and IXamlMetadataProvider  but most likely
             * won't be doing that for this assignment.
             */
            ViewModel = App.Services.GetRequiredService<MainViewModel>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.LoadDataAsync();
        }
    }
}
