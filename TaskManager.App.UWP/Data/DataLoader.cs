using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.App.UWP.Core.Models;
using TaskManagerCore.Controller;
using TaskManagerCore.Model.Dto;

namespace TaskManager.App.UWP.Data
{
    public class DataLoader : ObservableObject
    {
        private readonly TaskController TaskController;

        public ObservableCollection<TaskFolderViewObject> TaskFolderItems { get; private set; } = new ObservableCollection<TaskFolderViewObject>();

        public string LatestCreatedTaskId { get; set; } = "";

        public DataLoader(TaskController taskController)
        {
            TaskController = taskController;
        }

        public async Task<List<TaskFolderViewObject>> LoadDataAsync()
        {
            TaskFolderItems.Clear();

            var folders = await Task.Run(() => TaskController.GetTaskFolders());
            if (folders.Count == 0)
            {
                await Task.Run(() => TaskController.CreateTaskFolder(new CreateFolderDto("Default Folder")));

                folders = await Task.Run(() => TaskController.GetTaskFolders());
            }

            folders.Sort((x, y) => x.Name.CompareTo(y.Name));

            var tfvos = new List<TaskFolderViewObject>();
            foreach (var folder in folders)
            {
                var ttfvo = new TaskFolderViewObject
                {
                    Name = folder.Name
                };

                //var _tasks = await Task.WhenAll(folder.TaskIds
                //    .Select((taskId) => Task.Run(() => TaskController.GetTask(taskId))));

                var taskIds = folder.TaskIds.ToArray();

                var _tasks = taskIds.Select(taskId => TaskController.GetTask(taskId)).ToArray();
                var tasks = _tasks.ToList();
                tasks.Sort((x, y) => x.Description.CompareTo(y.Description));

                foreach (var task in _tasks)
                {
                    var streak = 0;
                    if (task.XData.TryGetValue("streak", out var streakStr))
                    {
                        int.TryParse(streakStr, out streak);
                    }    

                    var ttvo = new TaskViewObject
                    {
                        Type = task.Type,
                        GlobalId = task.Id,
                        Description = task.Description,
                        Notes = task.Notes,
                        DueDate = task.DueDate,
                        Completed = task.Completed,
                        Overdue = task.Overdue,
                        ParentFolderName = folder.Name,
                        Streak = streak
                    };

                    ttfvo.Tasks.Add(ttvo);
                }

                // sort for overdue first and by date after
                ttfvo.Tasks = ttfvo.Tasks
                        .GroupBy(t => t.Overdue) 
                        .OrderByDescending(g => g.Key)     
                        .SelectMany(g => g.OrderBy(t => t.DueDate))
                        .ToList();

                tfvos.Add(ttfvo);
            }

            foreach (var item in tfvos)
            {
                TaskFolderItems.Add(item);
            }

            return tfvos;
        }
    }
}
