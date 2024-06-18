using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TaskManager.App.UWP.Core.Models;
using TaskManager.App.UWP.ViewModels;
using Windows.UI;
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

        public async void TaskNotesEditBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ViewModel.SaveTaskInvokedCommand.Execute(null);
        }

        public async void TaskListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedItem = (TaskViewObject)e.ClickedItem; 
            if (selectedItem != null)
            {
                ViewModel.SelectedItem = selectedItem;
            }
        }

        public async void TaskNotesChanged(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                string newText = textBox.Text;
                ViewModel.UpdateTaskNotesInvokedCommand.Execute(newText);
            }
        }

        #region Dialog display methods

        public async void NewFolderAction(object sender, RoutedEventArgs e)
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

        public async void NewTaskAction(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                string parentFolderId = (string)button.CommandParameter;
                Debug.WriteLine($"Triggering New Task in folder {parentFolderId}");

                var newTaskDialog = new NewTaskDialog();
                newTaskDialog.SetParentFolderId(parentFolderId);
                await newTaskDialog.ShowAsync();

                ViewModel.NewTaskInvokedCommand.Execute(parentFolderId);
            }
        }

        public async void DeleteFolderAction(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                string folderId = (string)button.CommandParameter;
                Debug.WriteLine($"Triggering Delete Folder {folderId}");

                await ShowDeleteFolderDialog(folderId);
            }
        }

        public async void DeleteTaskAction(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                string taskId = (string)button.CommandParameter;
                Debug.WriteLine($"Triggering Delete Task {taskId}");

                await ShowDeleteTaskDialog(taskId);

                //await ViewModel.LoadDataAsync();
            }
        }

        public async void CompleteTaskAction(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                string taskId = (string)button.CommandParameter;
                Debug.WriteLine($"Triggering Complete Task {taskId}");

                TaskIdTextbox.Text = taskId;
                var result = await CompleteTaskDialog.ShowAsync();
                Debug.WriteLine($"Clicked: {result}");
                TaskIdTextbox.Text = string.Empty;

                //await ShowCompleteTaskDialog(taskId);
                //await ViewModel.LoadDataAsync();
            }
        }

        public async void DuplicateTaskAction(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                string taskId = (string)button.CommandParameter;
                Debug.WriteLine($"Triggering Edit Task {taskId}");

                //await ViewModel.LoadDataAsync();
            }
        }

        public async void MoveTaskAction(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                string taskId = (string)button.CommandParameter;
                Debug.WriteLine($"Triggering Move Task {taskId}");

                //await ViewModel.LoadDataAsync();
            }
        }

        public async void RenameFolderAction(object sender, RoutedEventArgs e)
        {
            var result = await RenameFolderDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                Debug.WriteLine($"Create new folder!");
            }
            else
            {
                Debug.WriteLine($"Create new folder cancelled!");
            }

            RenameFolderTextBox.Text = string.Empty;
        }

        #endregion

        private async Task ShowDeleteFolderDialog(string folderId)
        {
            ContentDialog confirmDialog = new ContentDialog
            {
                Title = "Confirmation",
                Content = "Are you sure you want to Delete this Folder?",
                PrimaryButtonText = "Cancel",
                SecondaryButtonText = "Delete Folder"
            };
            var redStyle = new Style(typeof(Button));
            redStyle.Setters.Add(new Setter(BackgroundProperty, Colors.Red));
            redStyle.Setters.Add(new Setter(ForegroundProperty, Colors.White));
            confirmDialog.SecondaryButtonStyle = redStyle;

            var result = await confirmDialog.ShowAsync();
            if (result == ContentDialogResult.Secondary)
            {
                Debug.WriteLine("Deleting folder...");
                ViewModel.DeleteFolderInvokedCommand.Execute(folderId);
            }
            else
            {
                Debug.WriteLine("Cancelled deletion!");
            }
        }

        private async Task ShowDeleteTaskDialog(string taskId)
        {
            ContentDialog confirmDialog = new ContentDialog
            {
                Title = "Confirmation",
                Content = "Are you sure you want to Delete this Task?",
                PrimaryButtonText = "Cancel",
                SecondaryButtonText = "Delete Task"
            };
            var redStyle = new Style(typeof(Button));
            redStyle.Setters.Add(new Setter(BackgroundProperty, Colors.Red));
            redStyle.Setters.Add(new Setter(ForegroundProperty, Colors.White));
            confirmDialog.SecondaryButtonStyle = redStyle;

            var result = await confirmDialog.ShowAsync();
            if (result == ContentDialogResult.Secondary)
            {
                Debug.WriteLine("Deleting task...");
                ViewModel.DeleteTaskInvokedCommand.Execute(taskId);
            }
            else
            {
                Debug.WriteLine("Cancelled deletion!");
            }
        }

        //private async Task ShowCompleteTaskDialog(string taskId)
        //{
        //    ContentDialog confirmDialog = new ContentDialog
        //    {
        //        Title = "Confirmation",
        //        Content = "Do you want to mark this task as Completed?",
        //        PrimaryButtonText = "Yes",
        //        SecondaryButtonText = "No",
        //        DefaultButton = ContentDialogButton.Primary
        //    };

        //    var result = await confirmDialog.ShowAsync();
        //    if (result == ContentDialogResult.Primary)
        //    {
        //        Debug.WriteLine("Completing task...");
        //        ViewModel.CompleteTaskInvokedCommand.Execute(taskId);
        //    }
        //    else
        //    {
        //        Debug.WriteLine("Cancelled deletion!");
        //    }
        //}

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
