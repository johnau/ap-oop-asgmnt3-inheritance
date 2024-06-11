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

        public TreeViewViewModel(TaskController taskController, DataLoader dataLoader)
        {
            TaskController = taskController;
            Data = dataLoader;
        }

        public async Task LoadDataAsync()
        {
            await Data.LoadDataAsync();
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

                await Data.LoadDataAsync();
            } catch (Exception ex)
            {
                Debug.WriteLine($"Could not create folder: {ex.Message}");
                // notify user with popup
            }
        }

        private async void OnCreateNewTaskInvoked(string arg)
        {
            //Debug.WriteLine($"Creating new folder with name: {taskDto.Description}");
            try
            {


                await Data.LoadDataAsync();
            } catch (Exception ex)
            {
                Debug.WriteLine($"Could not create task: {ex.Message}");
            }
        }
    }
}
