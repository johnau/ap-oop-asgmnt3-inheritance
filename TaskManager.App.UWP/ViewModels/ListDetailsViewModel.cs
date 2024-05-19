using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp.UI.Controls;

using TaskManager.App.UWP.Core.Models;
using TaskManager.App.UWP.Core.Services;
using TaskManagerCore.Controller;
using TaskManagerCore.Model.Dto;

namespace TaskManager.App.UWP.ViewModels
{
    public class ListDetailsViewModel : ObservableObject
    {
        private readonly TaskController TaskController;

        public TempTaskFolderViewObject Selected
        {
            get { return _selected; }
            set { SetProperty(ref _selected, value); }
        }
        private TempTaskFolderViewObject _selected;

        public ObservableCollection<TempTaskFolderViewObject> TestingItems { get; private set; } = new ObservableCollection<TempTaskFolderViewObject>();

        public ListDetailsViewModel(TaskController taskController)
        {
            TaskController = taskController;
        }

        public async Task LoadDataAsync(ListDetailsViewState viewState)
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

            if (viewState == ListDetailsViewState.Both)
            {
                Selected = TestingItems.First();
            }
        }
    }
}
