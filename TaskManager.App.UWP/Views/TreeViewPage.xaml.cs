using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using TaskManager.App.UWP.ViewModels;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
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

            SetTreeViewColumnMaxWidth();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.LoadDataAsync();
        }

        private async void ShowNewFolderDialog(object sender, RoutedEventArgs e)
        {
            var result = await CreateNewFolderDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                Debug.WriteLine($"Create new folder!");
            } else
            {
                Debug.WriteLine($"Create new folder cancelled!");
            }

            FolderNameTextBox.Text = string.Empty;
        }

        private async void ShowNewTaskDialog(object sender, RoutedEventArgs e)
        {
            var newTaskDialog = new NewTaskDialog();
            await newTaskDialog.ShowAsync();
            //var result = await CreateNewTaskDialog.ShowAsync();
            //if (result == ContentDialogResult.Primary)
            //{
            //    Debug.WriteLine($"Create new task!");
            //}
            //else
            //{
            //    Debug.WriteLine($"Create new task cancelled!");
            //}

            //FolderNameTextBox.Text = string.Empty;
        }

        private void MainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetTreeViewColumnMaxWidth();
        }

        private void TreeView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ViewModel.TreeViewIsWide = e.NewSize.Width > 350d;
        }

        private void DetailsPane_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ViewModel.DetailsViewIsWide = e.NewSize.Width > 450d;
        }

        private void SetTreeViewColumnMaxWidth()
        {
            double parentWidth = Window.Current.Bounds.Width;
            double maxColumnWidth = parentWidth * 0.4;
            treeViewColumn.MaxWidth = maxColumnWidth;
        }
    }
}
