using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using NaturalLanguageProcessor;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using TaskManager.App.UWP.Core.Models;
using TaskManager.App.UWP.Data;
using TaskManager.App.UWP.Helpers;
using TaskManager.App.UWP.Services;
using TaskManager.App.UWP.Views;
using TaskManagerCore.Controller;
using TaskManagerCore.Model;
using TaskManagerCore.Model.Dto;
using Windows.UI.Xaml.Controls;

namespace TaskManager.App.UWP.ViewModels
{
    public class MainViewModel : ObservableObject, INotifyPropertyChanged
    {
        private readonly TaskController TaskController;
        private readonly ForgivingFormatWithRegexProcessor NLP;
        public readonly DataLoader Data;

        private TaskViewObject _interpretedTask;
        private TimeSpan _defaultTaskTime = TimeSpan.Zero;

        public object SelectedTaskFolder
        {
            get => _selectedTaskFolder;
            set => SetProperty(ref _selectedTaskFolder, value);
        }
        private object _selectedTaskFolder;

        private string _taskInput;
        public string TaskInput
        {
            get => _taskInput;
            set => SetProperty(ref _taskInput, value);
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ICommand CreateTaskCommand { get; }

        public MainViewModel(TaskController taskController, DataLoader dataLoader)
        {
            TaskController = taskController;
            Data = dataLoader;
            CreateTaskCommand = new RelayCommand(CreateTask);

            NLP = new ForgivingFormatWithRegexProcessor();
        }

        public async Task LoadDataAsync()
        {
            await Data.LoadDataAsync();

            if (Data.TaskFolderItems.Count > 0)
            {
                SelectedTaskFolder = Data.TaskFolderItems.First();
            }

            await DefaultTaskTimeService.InitializeAsync();
            _defaultTaskTime = DefaultTaskTimeService.DefaultTaskTime;
        }

        private async void CreateTask()
        {
            Debug.WriteLine($"Create task {_taskInput} in folder: {((TaskFolderViewObject)SelectedTaskFolder).Name}");

            var parentFolderName = ((TaskFolderViewObject)_selectedTaskFolder).Name;

            var dueDate = _interpretedTask.DueDate;
            if (dueDate == null)
            {
                var currentDate = DateTime.Now;
                var targetDate = currentDate.Date.Add(_defaultTaskTime);
                if (targetDate < currentDate)
                {
                    targetDate = targetDate.AddDays(1);
                }

                dueDate = targetDate;
            }

            var createTaskDto = new CreateTaskDto(
                type:       TaskType.SINGLE,
                folderId:   parentFolderName,
                description:_interpretedTask.Description,
                notes:      string.Empty,
                dueDate:    dueDate
            );

            await Task.Run(() => TaskController.CreateTask(createTaskDto));
        }

        public void OnTaskInputChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            var newText = textBox.Text;

            var taskInfo = NLP.ProcessNaturalTask(newText);

            Debug.WriteLine($"NLP: {taskInfo.Description} {taskInfo.HasTime} {taskInfo.OccursAt}");

            StatusMessage = taskInfo.Description;

            if (taskInfo.HasTime)
            {
                //StatusMessage += $" @ {taskInfo.OccursAt:dd-MM-yyyy HH:mm}";
                StatusMessage += $" on {taskInfo.OccursAt:dddd d\\t\\h 'of' MMMM yyyy 'at' h:mmtt}";
            }

            var task = new TaskViewObject();
            task.ParentFolderName = ((TaskFolderViewObject)SelectedTaskFolder).Name;
            task.Description = taskInfo.Description;
            if (taskInfo.HasTime) task.DueDate = taskInfo.OccursAt;

            _interpretedTask = task;

            Debug.WriteLine($"The value of statusMessage is now: {_statusMessage}");
        }

    }
}
