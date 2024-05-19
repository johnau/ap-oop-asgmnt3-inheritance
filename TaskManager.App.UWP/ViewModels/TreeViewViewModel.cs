using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using TaskManager.App.UWP.Core.Models;
using TaskManager.App.UWP.Core.Services;
using TaskManagerCore.Controller;
using Windows.UI.Xaml.Controls.Primitives;
using WinUI = Microsoft.UI.Xaml.Controls;

namespace TaskManager.App.UWP.ViewModels
{
    public class TreeViewViewModel : ObservableObject
    {
        private readonly TaskController TaskController;

        private ICommand _itemInvokedCommand;
        
        public object SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }
        private object _selectedItem;

        public ObservableCollection<TempTaskFolderViewObject> TestingItems { get; private set; } = new ObservableCollection<TempTaskFolderViewObject>();

        public ICommand ItemInvokedCommand => _itemInvokedCommand ?? (_itemInvokedCommand = new RelayCommand<WinUI.TreeViewItemInvokedEventArgs>(OnItemInvoked));

        public TreeViewViewModel(TaskController taskController)
        {
            TaskController = taskController;
        }

        public async Task LoadDataAsync()
        {
            TestingItems.Clear();

            var folders = await Task.Run(() => TaskController.GetTaskFolders());
            var ttfvos = new List<TempTaskFolderViewObject>();

            foreach (var folder in folders)
            {
                var ttfvo = new TempTaskFolderViewObject();
                ttfvo.Name = folder.Name;

                var tasks = await Task.WhenAll(folder.TaskIds.Select(taskId => Task.Run(() => TaskController.GetTask(taskId))));

                foreach (var task in tasks)
                {
                    var ttvo = new TempTaskViewObject
                    {
                        GlobalId = task.Id,
                        Description = task.Description,
                        Notes = task.Notes,
                        DueDate = task.DueDate,
                        Completed = task.Completed,
                        Overdue = task.Overdue,
                        ParentFolderName = folder.Name
                    };

                    ttfvo.Tasks.Add(ttvo);
                }

                ttfvos.Add(ttfvo);
            }

            foreach (var item in ttfvos)
            {
                TestingItems.Add(item);
            }
        }

        private void OnItemInvoked(WinUI.TreeViewItemInvokedEventArgs args)
            => SelectedItem = args.InvokedItem;
    }
}
