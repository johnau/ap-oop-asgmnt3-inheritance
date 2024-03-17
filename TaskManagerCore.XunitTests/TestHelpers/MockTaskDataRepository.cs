using TaskManagerCore.Configuration;
using TaskManagerCore.Model;

namespace TaskManagerCore.XunitTests.TestHelpers
{
    public class MockTaskDataRepository : ICrudRepository<TaskData, string>
    {
        public Func<string, bool>? OnDelete { get; set; }
        public Func<List<TaskData>>? OnFindAll { get; set; }
        public Func<string, TaskData?>? OnFindById { get; set; }
        public Func<List<string>, List<TaskData>>? OnFindByIds { get; set; }
        public Func<TaskData, string?>? OnSave { get; set; }

        public bool Delete(string id)
        {
            return OnDelete != null ? OnDelete(id) : throw new NotImplementedException("Did not provide a function for testing");
        }

        public List<TaskData> FindAll()
        {
            return OnFindAll != null ? OnFindAll() : throw new NotImplementedException("Did not provide a function for testing");
        }

        public TaskData? FindById(string id)
        {
            return OnFindById != null ? OnFindById(id) : throw new NotImplementedException("Did not provide a function for testing");
        }

        public List<TaskData> FindByIds(List<string> ids)
        {
            return OnFindByIds != null ? OnFindByIds(ids) : throw new NotImplementedException("Did not provide a function for testing");
        }

        public string Save(TaskData o)
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
