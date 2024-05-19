using System.Collections.Generic;
using System.Diagnostics;
using TaskManagerCore.Model;
using TaskManagerCore.Model.Repository;

namespace TaskManagerCore.Infrastructure
{
    internal class TaskFolderDualRepositoryRunner : ITaskFolderRepository
    {
        private ITaskFolderRepository _taskFolderRepository1;
        private ITaskFolderRepository _taskFolderRepository2;

        public TaskFolderDualRepositoryRunner(ITaskFolderRepository taskFolderRepository1, ITaskFolderRepository taskFolderRepository2)
        {
            _taskFolderRepository1 = taskFolderRepository1;
            _taskFolderRepository2 = taskFolderRepository2;
        }

        public bool Delete(string id)
        {
            var result1 = _taskFolderRepository1.Delete(id);
            var result2 = _taskFolderRepository2.Delete(id);

            ComparePrimitive("deleted by id successfully", result1, result2);

            return result1;
        }

        public bool DeleteByName(string name)
        {
            var result1 = _taskFolderRepository1.DeleteByName(name);
            var result2 = _taskFolderRepository2.DeleteByName(name);

            ComparePrimitive("deleted by name successfully", result1, result2);

            return result1;
        }

        public List<TaskFolder> FindAll()
        {
            var result1 = _taskFolderRepository1.FindAll();
            var result2 = _taskFolderRepository2.FindAll();

            CompareMultiple(result1, result2);

            return result1;
        }

        public TaskFolder FindById(string id)
        {
            var result1 = _taskFolderRepository1.FindById(id);
            var result2 = _taskFolderRepository2.FindById(id);

            CompareSingle(result1, result2);

            return result1;
        }

        public List<TaskFolder> FindByIds(List<string> ids)
        {
            var result1 = _taskFolderRepository1.FindByIds(ids);
            var result2 = _taskFolderRepository2.FindByIds(ids);

            CompareMultiple(result1, result2);

            return result1;
        }

        public TaskFolder FindByName(string name)
        {
            var result1 = _taskFolderRepository1.FindByName(name);
            var result2 = _taskFolderRepository2.FindByName(name);

            CompareSingle(result1, result2);

            return result1;
        }

        public List<TaskFolder> FindByNameStartsWith(string name)
        {
            var result1 = _taskFolderRepository1.FindByNameStartsWith(name);
            var result2 = _taskFolderRepository2.FindByNameStartsWith(name);

            CompareMultiple(result1, result2);

            return result1;
        }

        public List<TaskFolder> FindEmpty()
        {
            var result1 = _taskFolderRepository1.FindEmpty();
            var result2 = _taskFolderRepository2.FindEmpty();

            CompareMultiple(result1, result2);

            return result1;
        }

        public List<TaskFolder> FindNotEmpty()
        {
            var result1 = _taskFolderRepository1.FindNotEmpty();
            var result2 = _taskFolderRepository2.FindNotEmpty();

            CompareMultiple(result1, result2);

            return result1;
        }

        public TaskFolder FindOneByName(string name)
        {
            var result1 = _taskFolderRepository1.FindOneByName(name);
            var result2 = _taskFolderRepository2.FindOneByName(name);

            CompareSingle(result1, result2);

            return result1;
        }

        public TaskFolder Save(TaskFolder o)
        {
            var result1 = _taskFolderRepository1.Save(o);

            //var forSql = EntityFactoryV2.WithId(o, result1);
            var result2 = _taskFolderRepository2.Save(result1);

            ComparePrimitive("id", result1, result2);

            return result1;
        }

        private void CompareMultiple(List<TaskFolder> list1, List<TaskFolder> list2)
        {
            for (int i = 0; i < list1.Count; i++)
            {
                var result1 = list1[i];
                var result2 = list2[i];
                if (!result1.Equals(result2))
                {
                    Debug.WriteLine($"✘ Sync issue with SQL database, results did not match for Folder: {result1.Name}");
                    return;
                }
            }

            Debug.WriteLine($"✔ Results match for BinaryFile and SQL database");
        }

        private void CompareSingle(TaskFolder a, TaskFolder b)
        {
            if (a.Equals(b))
                Debug.WriteLine($"✔ Results match for BinaryFile and SQL database");
            else
                Debug.WriteLine($"✘ Sync issue with SQL database, results did not match for Folder: {a.Name}");
        }

        private void ComparePrimitive(string name, object a, object b)
        {
            if (a == b)
                Debug.WriteLine($"✔ Results match for BinaryFile and SQL database");
            else
                Debug.WriteLine($"✘ Sync issue with SQL database, results did not match for Folder Primitive: {name}");
        }
    }
}
