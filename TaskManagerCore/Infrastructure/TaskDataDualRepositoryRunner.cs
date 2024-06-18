using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaskManagerCore.Model;
using TaskManagerCore.Model.Repository;

namespace TaskManagerCore.Infrastructure
{
    internal class TaskDataDualRepositoryRunner : ITaskDataRepository
    {
        private ITaskDataRepository _taskDataRepository1;
        private ITaskDataRepository _taskDataRepository2;

        public TaskDataDualRepositoryRunner(ITaskDataRepository taskDataRepository1, ITaskDataRepository taskDataRepository2)
        {
            _taskDataRepository1 = taskDataRepository1;
            _taskDataRepository2 = taskDataRepository2;
        }

        public bool Delete(string id)
        {
            var result1 = _taskDataRepository1.Delete(id);
            var result2 = _taskDataRepository2.Delete(id);

            ComparePrimitive("deleted successfully", result1, result2);

            return result1;
        }

        public List<TaskData> FindAll()
        {
            var result1 = _taskDataRepository1.FindAll();
            var result2 = _taskDataRepository2.FindAll();

            CompareMultiple(result1, result2);

            return result1;
        }

        public List<TaskData> FindByCompleted(bool completed)
        {
            var result1 = _taskDataRepository1.FindByCompleted(completed);
            var result2 = _taskDataRepository2.FindByCompleted(completed);

            CompareMultiple(result1, result2);

            return result1;
        }

        public List<TaskData> FindByDescription(string description)
        {
            var result1 = _taskDataRepository1.FindByDescription(description);
            var result2 = _taskDataRepository2.FindByDescription(description);

            CompareMultiple(result1, result2);

            return result1;
        }

        public List<TaskData> FindByDueDate(DateTime dueDate)
        {
            var result1 = _taskDataRepository1.FindByDueDate(dueDate);
            var result2 = _taskDataRepository2.FindByDueDate(dueDate);

            CompareMultiple(result1, result2);

            return result1;
        }

        public TaskData FindById(string id)
        {
            var result1 = _taskDataRepository1.FindById(id);
            var result2 = _taskDataRepository2.FindById(id);

            CompareSingle(result1, result2);

            return result1;
        }

        public List<TaskData> FindByIds(List<string> ids)
        {
            var result1 = _taskDataRepository1.FindByIds(ids);
            var result2 = _taskDataRepository2.FindByIds(ids);

            CompareMultiple(result1, result2);

            return result1;
        }

        public List<TaskData> FindByNotes(string notes)
        {
            var result1 = _taskDataRepository1.FindByNotes(notes);
            var result2 = _taskDataRepository2.FindByNotes(notes);

            CompareMultiple(result1, result2);

            return result1;
        }

        public TaskData Save(TaskData o)
        {
            var result1 = _taskDataRepository1.Save(o);

            //var forSql = EntityFactory.WithId(o, result1);
            var result2 = _taskDataRepository2.Save(result1);

            CompareSingle(result1, result2);

            return result1;
        }

        private void CompareMultiple(List<TaskData> list1, List<TaskData> list2)
        {
            var _list1 = list1.OrderBy(x => x.Description).ToList();
            var _list2 = list2.OrderBy(x => x.Description).ToList();

            for (int i = 0; i < _list1.Count; i++)
            {
                var result1 = _list1[i];
                var result2 = _list2[i];
                if (!result1.Equals(result2))
                {
                    Debug.WriteLine($"✘ Sync issue with SQL database, results did not match for Task: {result1.Description}");
                    return;
                }
            }

            Debug.WriteLine($"✔ Results match for BinaryFile and SQL database");
        }

        private void CompareSingle(TaskData a, TaskData b)
        {
            if (a == null && b == null)
                Debug.WriteLine($"✔ Results match for BinaryFile and SQL database (both Null)");
            else if (a == null && b != null)
                Debug.WriteLine($"✘ Sync issue with SQL database, results did not match for Task (a was null, b was not null)");
            else if (a != null && b == null)
                Debug.WriteLine($"✘ Sync issue with SQL database, results did not match for Task (a was not null, b was null)");
            else if (a.Equals(b))
                Debug.WriteLine($"✔ Results match for BinaryFile and SQL database");
            else
                Debug.WriteLine($"✘ Sync issue with SQL database, results did not match for Task: {a.Description}");
        }

        private void ComparePrimitive(string name, object a, object b)
        {
            if (a == b)
                Debug.WriteLine($"✔ Results match for BinaryFile and SQL database");
            else
                Debug.WriteLine($"✘ Sync issue with SQL database, results did not match for Task Primitive: {name}");
        }
    }
}
