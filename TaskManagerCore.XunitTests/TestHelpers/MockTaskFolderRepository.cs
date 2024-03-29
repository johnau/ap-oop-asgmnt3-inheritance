using TaskManagerCore.Configuration;
using TaskManagerCore.Model;
using TaskManagerCore.Model.Repository;

namespace TaskManagerCore.XunitTests.TestHelpers
{
    internal class MockTaskFolderRepository : ITaskFolderRepository
    {
        public Func<string, bool>? OnDelete { get; set; }
        public Func<List<TaskFolder>>? OnFindAll { get; set; }
        public Func<string, TaskFolder?>? OnFindById { get; set; }
        public Func<List<string>, List<TaskFolder>>? OnFindByIds { get; set; }
        public Func<TaskFolder, string?>? OnSave { get; set; }

        public bool Delete(string id)
        {
            return OnDelete != null ? OnDelete(id) : throw new NotImplementedException("Did not provide a function for testing");
        }

        public bool DeleteByName(string name)
        {
            throw new NotImplementedException();
        }

        public List<TaskFolder> FindAll()
        {
            return OnFindAll != null ? OnFindAll() : throw new NotImplementedException("Did not provide a function for testing");
        }

        public TaskFolder? FindById(string id)
        {
            return OnFindById != null ? OnFindById(id) : throw new NotImplementedException("Did not provide a function for testing");
        }

        public List<TaskFolder> FindByIds(List<string> ids)
        {
            return OnFindByIds != null ? OnFindByIds(ids) : throw new NotImplementedException("Did not provide a function for testing");
        }

        public TaskFolder? FindByName(string name)
        {
            throw new NotImplementedException();
        }

        public List<TaskFolder> FindByNameStartsWith(string name)
        {
            throw new NotImplementedException();
        }

        public List<TaskFolder> FindEmpty()
        {
            throw new NotImplementedException();
        }

        public List<TaskFolder> FindNotEmpty()
        {
            throw new NotImplementedException();
        }

        public TaskFolder? FindOneByName(string name)
        {
            throw new NotImplementedException();
        }

        public string Save(TaskFolder o)
        {
            #pragma warning disable CS8603 // Possible null reference return.
            return OnSave != null ? OnSave(o) : throw new NotImplementedException("Did not provide a function for testing");
            #pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
