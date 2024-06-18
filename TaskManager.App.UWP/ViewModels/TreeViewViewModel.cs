using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using TaskManager.App.UWP.Core.Models;
using TaskManager.App.UWP.Core.Services;
using TaskManager.App.UWP.Data;
using TaskManagerCore.Controller;
using TaskManagerCore.Model.Dto;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using WinUI = Microsoft.UI.Xaml.Controls;

namespace TaskManager.App.UWP.ViewModels
{
    public class TreeViewViewModel : ObservableObject
    {
        private readonly TaskController TaskController;
        public readonly DataLoader Data;

        public ICommand ItemInvokedCommand => _itemInvokedCommand ?? (_itemInvokedCommand = new RelayCommand<WinUI.TreeViewItemInvokedEventArgs>(OnItemInvoked));
        private ICommand _itemInvokedCommand;

        public ICommand NewFolderInvokedCommand => _newFolderInvokedCommand ?? (_newFolderInvokedCommand = new RelayCommand<string>(OnCreateNewFolderInvoked));
        private ICommand _newFolderInvokedCommand;

        public ICommand NewTaskInvokedCommand => _newTaskInvokedCommand ?? (_newTaskInvokedCommand = new RelayCommand<string>(OnCreateNewTaskInvoked));
        private ICommand _newTaskInvokedCommand;

        public ICommand DeleteTaskInvokedCommand => _deleteTaskInvokedCommand ?? (_deleteTaskInvokedCommand = new RelayCommand<string>(OnDeleteTaskInvoked));
        private ICommand _deleteTaskInvokedCommand;

        public ICommand DeleteFolderInvokedCommand => _deleteFolderInvokedCommand ?? (_deleteFolderInvokedCommand = new RelayCommand<string>(OnDeleteFolderInvoked));
        private ICommand _deleteFolderInvokedCommand;

        public ICommand CompleteTaskInvokedCommand => _completeTaskInvokedCommand ?? (_completeTaskInvokedCommand = new RelayCommand<string>(OnCompleteTaskInvoked));
        private ICommand _completeTaskInvokedCommand;

        public ICommand RenameFolderInvokedCommand => _renameFolderInvokedCommand ?? (_renameFolderInvokedCommand = new RelayCommand<string>(OnRenameFolderInvoked));
        private ICommand _renameFolderInvokedCommand;

        public ICommand UpdateTaskNotesInvokedCommand => _updateTaskNotesInvokedCommand ?? (_updateTaskNotesInvokedCommand = new RelayCommand<string>(UpdateTaskNotesInvoked));
        private ICommand _updateTaskNotesInvokedCommand;

        public ICommand SaveTaskInvokedCommand => _savedTaskInvokedCommand ?? (_savedTaskInvokedCommand = new RelayCommand(OnSaveTaskInvoked));
        private ICommand _savedTaskInvokedCommand;

        /// <summary>
        /// ObservableProperty to track the select (folder/task) for the view
        /// </summary>
        public object SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }
        private object _selectedItem;

        /// <summary>
        /// Observable propety to track List Pane view size
        /// </summary>
        public bool TreeViewIsWide
        {
            get => _iconsOnly;
            set => SetProperty(ref _iconsOnly, value);
        }
        private bool _iconsOnly;

        /// <summary>
        /// Observable property to track Details Pane view size
        /// </summary>
        public bool DetailsViewIsWide
        {
            get => _detailsViewIsWide;
            set => SetProperty(ref _detailsViewIsWide, value);
        }
        private bool _detailsViewIsWide;

        public bool TaskIsSelected
        {
            get => _taskIsSelected;
            set => SetProperty(ref _taskIsSelected, value);
        }
        private bool _taskIsSelected = false;

        public bool FolderIsSelected
        {
            get => _folderIsSelected;
            set => SetProperty(ref _folderIsSelected, value);
        }
        private bool _folderIsSelected = false;

        public string LatestCreatedTask
        {
            get => _latestCreatedTask;
            set => SetProperty(ref _latestCreatedTask, value);
        }
        private string _latestCreatedTask;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="taskController"></param>
        /// <param name="dataLoader"></param>
        public TreeViewViewModel(TaskController taskController, DataLoader dataLoader)
        {
            TaskController = taskController;
            Data = dataLoader;
            PropertyChanged += TreeViewViewModel_PropertyChanged;
        }

        private void TreeViewViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem" && SelectedItem != null)
            {
                if (SelectedItem.GetType() == typeof(TaskFolderViewObject))
                {
                    FolderIsSelected = true;
                    TaskIsSelected = false;
                } else if (SelectedItem.GetType() == typeof(TaskViewObject))
                {
                    FolderIsSelected = false;
                    TaskIsSelected = true;
                }
            }
        }

        public async Task LoadDataAsync()
        {
            await Data.LoadDataAsync();
            //SelectedItem = Data.TaskFolderItems.FirstOrDefault();
            //SelectedItem = null;
        }

        private void OnItemInvoked(WinUI.TreeViewItemInvokedEventArgs args)
            => SelectedItem = args.InvokedItem;

        private async void OnCreateNewFolderInvoked(string newFolderName)
        {
            Debug.WriteLine($"Creating new folder with name: {newFolderName}");
            try
            {
                var createdId = TaskController.CreateTaskFolder(new CreateFolderDto(newFolderName));
                Debug.WriteLine($"ID of created folder={createdId}");

                var currentSelection = SelectedItem;
                await Data.LoadDataAsync();

                SelectedItem = currentSelection;
            } catch (Exception ex)
            {
                Debug.WriteLine($"Could not create folder: {ex.Message}");
                // notify user with popup
            }
        }

        private async void OnCreateNewTaskInvoked(string arg)
        {
            try
            {
                await Data.LoadDataAsync();

                SelectedItem = Data.TaskFolderItems.SelectMany(f => f.Tasks).Where(t => t.GlobalId == Data.LatestCreatedTaskId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could not create task: {ex.Message}");
            }
        }

        private async void OnDeleteTaskInvoked(string taskId)
        {
            Debug.WriteLine($"Deleting task: {taskId}");
            _ = TaskController.DeleteTask(taskId);

            await LoadDataAsync();
            SelectedItem = Data.TaskFolderItems.FirstOrDefault();
        }

        private async void OnDeleteFolderInvoked(string folderId)
        {
            Debug.WriteLine($"Deleting task: {folderId}");
            _ = TaskController.DeleteTaskFolder(folderId);

            var currentSelection = SelectedItem;
            await LoadDataAsync();

            SelectedItem = currentSelection;
        }

        private async void OnCompleteTaskInvoked(string taskId)
        {
            Debug.WriteLine($"Completing task: {taskId}");
            _ = TaskController.CompleteTask(taskId);

            var currentSelection = (TaskViewObject) SelectedItem;
            
            await LoadDataAsync();

            SelectedItem = Data.TaskFolderItems.SelectMany(f => f.Tasks).Where(t => t.GlobalId == currentSelection.GlobalId).FirstOrDefault();
        }

        private async void OnRenameFolderInvoked(string folderId)
        {
            Debug.WriteLine($"Renaming folder: {folderId}");

            var currentSelection = SelectedItem;
            await LoadDataAsync();

            SelectedItem = currentSelection;
        }

        private async void OnSaveTaskInvoked()
        {
            var task = (TaskViewObject)SelectedItem;

            //TaskController.UpdateTaskProperty(task.GlobalId, "notes", );

            await LoadDataAsync();

            SelectedItem = Data.TaskFolderItems.SelectMany(f => f.Tasks).Where(t => t.GlobalId == task.GlobalId).FirstOrDefault();
        }

        private async void UpdateTaskNotesInvoked(string notes)
        {
            var task = (TaskViewObject)SelectedItem;

            TaskController.UpdateTaskProperty(task.GlobalId, "notes", notes);
        }
    }
}
