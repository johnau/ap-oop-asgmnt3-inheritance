using Microsoft.Extensions.DependencyInjection;
using System;

using TaskManager.App.UWP.ViewModels;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TaskManager.App.UWP.Views
{
    public sealed partial class ListDetailsPage : Page
    {
        public ListDetailsViewModel ViewModel { get; }

        public ListDetailsPage()
        {
            InitializeComponent();
            Loaded += ListDetailsPage_Loaded;

            /**
             * Dependency Injection Note:
             * UWP Apps don't allow constructors with parameters, so we must manually 
             * get the dependency.  
             * There is a workaround by a custom implementation of 
             * IXamlType.ActivateInstance() and IXamlMetadataProvider  but most likely
             * won't be doing that for this assignment.
             */
            ViewModel = App.Services.GetRequiredService<ListDetailsViewModel>(); 
        }

        private async void ListDetailsPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadDataAsync(ListDetailsViewControl.ViewState);
        }
    }
}
