using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TaskManager.App.UWP.Data;
using TaskManagerCore.Controller;
using TaskManagerCore.Model;
using TaskManagerCore.Model.Dto;

namespace TaskManager.App.UWP.ViewModels
{
    public class NewTaskDialogViewModel : ObservableObject, INotifyPropertyChanged
    {
        private readonly TaskController TaskController;
        private readonly DataLoader Data;

        // Probably shouldn't be observable? regular list as it won't change
        private readonly ObservableCollection<string> _timeIntervals = new ObservableCollection<string>()
        {
            TimeInterval.Hourly.ToString(),
            TimeInterval.Daily.ToString(),
            TimeInterval.Weekly.ToString(),
            TimeInterval.Fortnightly.ToString(),
            TimeInterval.Monthly.ToString(),
            TimeInterval.Yearly.ToString()
        };
        public ObservableCollection<string> TimeIntervals
        {
            get => _timeIntervals;
            set => _ = value;
        }

        public ICommand NewTaskInvokedCommand => _newTaskInvokedCommand ?? (_newTaskInvokedCommand = new RelayCommand<string>(OnNewTaskInvoked));
        private ICommand _newTaskInvokedCommand;

        public string TaskDescription
        {
            get => _taskDescription;
            set => SetProperty(ref _taskDescription, value);
        }
        private string _taskDescription;

        public string TaskNotes
        {
            get => _taskNotes;
            set => SetProperty(ref _taskNotes, value);
        }
        private string _taskNotes;

        public bool TaskHasDueDate
        {
            get => _taskHasDueDate;
            set => SetProperty(ref _taskHasDueDate, value);
        }
        private bool _taskHasDueDate;

        public DateTimeOffset SelectedDate
        {
            get => _selectedDate;
            set => SetProperty(ref _selectedDate, value);
        }
        private DateTimeOffset _selectedDate;

        public TimeSpan SelectedTime
        {
            get => _selectedTime;
            set => SetProperty(ref _selectedTime, value);
        }
        private TimeSpan _selectedTime;

        public string ParentFolderId
        {
            get => _folderId;
            set
            {
                Debug.WriteLine($"Updated folder id in dialog to : {value}");
                SetProperty(ref _folderId, value);
            }
        }
        private string _folderId;

        public int TaskTypeIndex
        {
            get => _taskTypeIndex;
            set
            {
                TaskHasInterval = value > 0;
                SetProperty(ref _taskTypeIndex, value);
            }
        }
        private int _taskTypeIndex;

        public bool TaskHasInterval
        {
            get => _taskHasInterval;
            set => SetProperty(ref _taskHasInterval, value);
        }
        private bool _taskHasInterval;

        public int SelectedTaskIntervalIndex
        {
            get => _selectedTaskIntervalIndex;
            set => SetProperty(ref _selectedTaskIntervalIndex, value);
        }
        private int _selectedTaskIntervalIndex;

        /// <summary>
        /// Primary constructor
        /// </summary>
        /// <param name="taskController"></param>
        public NewTaskDialogViewModel(TaskController taskController, DataLoader dataLoader)
        {
            TaskController = taskController;
            Data = dataLoader;
            _folderId = "<Folder name>";
            _taskNotes = "";
            _taskHasDueDate = false;

            SelectedDate = DateTimeOffset.Now;
        }

        private async void OnNewTaskInvoked(string taskDescription)
        {
            DateTime? dueDate = null;
            if (TaskHasDueDate) {
                DateTimeOffset combined = new DateTimeOffset(
                    SelectedDate.Year, SelectedDate.Month, SelectedDate.Day,
                    SelectedTime.Hours, SelectedTime.Minutes, SelectedTime.Seconds, TimeSpan.Zero);
                dueDate = combined.DateTime;
            }

            var timeInterval = TimeInterval.None.ToString();
            if (SelectedTaskIntervalIndex - 1 <= TimeIntervals.Count)
                timeInterval = TimeIntervals[SelectedTaskIntervalIndex];

            var taskType = TaskType.SINGLE;
            if (TaskTypeIndex == 1)
            {
                taskType = TaskType.REPEATING;
            } else if (TaskTypeIndex == 2)
            {
                taskType = TaskType.REPEATING_STREAK;
            }

            if (!Enum.TryParse<TimeInterval>(timeInterval, out var timeIntervalValue))
            {
                timeIntervalValue = TimeInterval.None;
            }

            Debug.WriteLine($"New task of type: '{taskType}': '{taskDescription}', Notes: '{TaskNotes}', TaskHasDueDate: '{TaskHasDueDate}' in parent folder: '{ParentFolderId}', DateTime:{dueDate}, Int: '{timeIntervalValue}'");

            var createDto = new CreateTaskDto(taskType, ParentFolderId, taskDescription, TaskNotes, dueDate, timeIntervalValue);

            var createdTaskId = "";
            await Task.Run(() => createdTaskId = TaskController.CreateTask(createDto));

            Debug.WriteLine($"Created task with id: {createdTaskId}");
            Data.LatestCreatedTaskId = createdTaskId;

            //await Data.LoadDataAsync();
        }
    }
}
