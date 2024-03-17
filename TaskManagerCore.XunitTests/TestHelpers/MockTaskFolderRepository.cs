using TaskManagerCore.Configuration;
using TaskManagerCore.Model;

namespace TaskManagerCore.XunitTests.TestHelpers
{
    internal class MockTaskFolderRepository : ICrudRepository<TaskFolder, string>
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

        public string Save(TaskFolder o)
        {
            if (OnSave == null) 
                throw new NotImplementedException("Did not provide a function for testing");
            
            var response = OnSave(o);
            
            if (response == null) 
                return "";
            
            return response;

        }
    }
}
