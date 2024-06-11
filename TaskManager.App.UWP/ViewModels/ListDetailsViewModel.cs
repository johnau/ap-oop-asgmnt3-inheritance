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
using TaskManager.App.UWP.Data;
using TaskManagerCore.Controller;
using TaskManagerCore.Model.Dto;

namespace TaskManager.App.UWP.ViewModels
{
    public class ListDetailsViewModel : ObservableObject
    {
        private readonly TaskController TaskController;
        public readonly DataLoader Data;

        public TaskFolderViewObject Selected
        {
            get { return _selected; }
            set { SetProperty(ref _selected, value); }
        }
        private TaskFolderViewObject _selected;

        public ListDetailsViewModel(TaskController taskController, DataLoader dataLoader)
        {
            TaskController = taskController;
            Data = dataLoader;
        }

        public async Task LoadDataAsync(ListDetailsViewState viewState)
        {
            await Data.LoadDataAsync();

            if (viewState == ListDetailsViewState.Both && Data.TaskFolderItems.Count != 0)
            {
                Selected = Data.TaskFolderItems.First();
            }
        }
    }
}
