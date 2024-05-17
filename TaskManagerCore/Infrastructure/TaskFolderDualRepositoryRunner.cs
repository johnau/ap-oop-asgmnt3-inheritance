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

            // compare results

            return result1;
        }

        public bool DeleteByName(string name)
        {
            var result1 = _taskFolderRepository1.DeleteByName(name);
            var result2 = _taskFolderRepository2.DeleteByName(name);

            // compare results

            return result1;
        }

        public List<TaskFolder> FindAll()
        {
            var result1 = _taskFolderRepository1.FindAll();
            var result2 = _taskFolderRepository2.FindAll();

            // compare results

            return result1;
        }

        public TaskFolder? FindById(string id)
        {
            var result1 = _taskFolderRepository1.FindById(id);
            var result2 = _taskFolderRepository2.FindById(id);

            // compare results

            return result1;
        }

        public List<TaskFolder> FindByIds(List<string> ids)
        {
            var result1 = _taskFolderRepository1.FindByIds(ids);
            var result2 = _taskFolderRepository2.FindByIds(ids);

            // compare results

            return result1;
        }

        public TaskFolder? FindByName(string name)
        {
            var result1 = _taskFolderRepository1.FindByName(name);
            var result2 = _taskFolderRepository2.FindByName(name);

            // compare results

            return result1;
        }

        public List<TaskFolder> FindByNameStartsWith(string name)
        {
            var result1 = _taskFolderRepository1.FindByNameStartsWith(name);
            var result2 = _taskFolderRepository2.FindByNameStartsWith(name);

            // compare results

            return result1;
        }

        public List<TaskFolder> FindEmpty()
        {
            var result1 = _taskFolderRepository1.FindEmpty();
            var result2 = _taskFolderRepository2.FindEmpty();

            // compare results

            return result1;
        }

        public List<TaskFolder> FindNotEmpty()
        {
            var result1 = _taskFolderRepository1.FindNotEmpty();
            var result2 = _taskFolderRepository2.FindNotEmpty();

            // compare results

            return result1;
        }

        public TaskFolder? FindOneByName(string name)
        {
            var result1 = _taskFolderRepository1.FindOneByName(name);
            var result2 = _taskFolderRepository2.FindOneByName(name);

            // compare results

            return result1;
        }

        public string Save(TaskFolder o)
        {
            var result1 = _taskFolderRepository1.Save(o);
            var result2 = _taskFolderRepository2.Save(o);

            // compare results

            return result1;
        }
    }
}
