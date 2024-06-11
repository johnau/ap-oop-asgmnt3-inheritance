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
using TaskManager.App.UWP.Data;
using TaskManager.App.UWP.Helpers;
using TaskManager.App.UWP.Views;
using TaskManagerCore.Controller;
using Windows.UI.Xaml.Controls;

namespace TaskManager.App.UWP.ViewModels
{
    public class MainViewModel : ObservableObject, INotifyPropertyChanged
    {
        private readonly TaskController TaskController;
        private readonly ForgivingFormatWithRegexProcessor NLP;
        public readonly DataLoader Data;

        public object SelectedTaskFolder
        {
            get => _selectedTaskFolder;
            set => SetProperty(ref _selectedTaskFolder, value);
        }
        private object _selectedTaskFolder;

        public MainViewModel(TaskController taskController, DataLoader dataLoader)
        {
            TaskController = taskController;
            Data = dataLoader;
            CreateTaskCommand = new RelayCommand(CreateTask);

            NLP = new ForgivingFormatWithRegexProcessor();
        }

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

        public async Task LoadDataAsync()
        {
            await Data.LoadDataAsync();

            if (Data.TaskFolderItems.Count > 0)
            {
                SelectedTaskFolder = Data.TaskFolderItems.First();
            }
        }

        private async void CreateTask()
        {
            //if (response?.Id != null)
            //{
            //    // Redirect to another view to show the created task
            //    // This could be done via navigation logic appropriate to your UWP app
            //}
            //else
            //{
            //    StatusMessage = "Input not recognized, Please try again";
            //}
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

            Debug.WriteLine($"The value of statusMessage is now: {_statusMessage}");
        }

    }
}
