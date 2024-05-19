using System;
using System.Collections.Generic;
using TaskManagerCore.Configuration;
using TaskManagerCore.Model;
using TaskManagerCore.Model.Repository;

namespace TaskManagerCore.XunitTests.TestHelpers
{
    public class MockTaskDataRepository : ITaskDataRepository
    {
        public Func<string, bool> OnDelete { get; set; }
        public Func<List<TaskData>> OnFindAll { get; set; }
        public Func<string, TaskData> OnFindById { get; set; }
        public Func<List<string>, List<TaskData>> OnFindByIds { get; set; }
        public Func<TaskData, string> OnSave { get; set; }

        public bool Delete(string id)
        {
            return OnDelete != null ? OnDelete(id) : throw new NotImplementedException("Did not provide a function for testing");
        }

        public List<TaskData> FindAll()
        {
            return OnFindAll != null ? OnFindAll() : throw new NotImplementedException("Did not provide a function for testing");
        }

        public List<TaskData> FindByCompleted(bool completed)
        {
            throw new NotImplementedException();
        }

        public List<TaskData> FindByDescription(string description)
        {
            throw new NotImplementedException();
        }

        public List<TaskData> FindByDueDate(DateTime dueDate)
        {
            throw new NotImplementedException();
        }

        public TaskData FindById(string id)
        {
            return OnFindById != null ? OnFindById(id) : throw new NotImplementedException("Did not provide a function for testing");
        }

        public List<TaskData> FindByIds(List<string> ids)
        {
            return OnFindByIds != null ? OnFindByIds(ids) : throw new NotImplementedException("Did not provide a function for testing");
        }

        public List<TaskData> FindByNotes(string notes)
        {
            throw new NotImplementedException();
        }

        public string Save(TaskData o)
        {
            #pragma warning disable CS8603 // Possible null reference return.
            return OnSave != null ? OnSave(o) : throw new NotImplementedException("Did not provide a function for testing");
            #pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
