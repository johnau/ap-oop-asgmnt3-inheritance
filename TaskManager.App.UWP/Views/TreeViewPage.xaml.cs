using Microsoft.Extensions.DependencyInjection;
using System;

using TaskManager.App.UWP.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace TaskManager.App.UWP.Views
{
    // For more info about the TreeView Control see
    // https://docs.microsoft.com/windows/uwp/design/controls-and-patterns/tree-view
    // For other samples, get the XAML Controls Gallery app http://aka.ms/XamlControlsGallery
    public sealed partial class TreeViewPage : Page
    {
        public TreeViewViewModel ViewModel { get; }

        public TreeViewPage()
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
            ViewModel = App.Services.GetRequiredService<TreeViewViewModel>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.LoadDataAsync();
        }
    }
}
