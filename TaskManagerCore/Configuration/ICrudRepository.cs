namespace TaskManagerCore.Configuration
{
    public interface ICrudRepository<T, ID> 
        where T : class
    {
        List<T> FindAll();
        List<T> FindByIds(List<ID> ids);
        T? FindById(ID id);
        ID Save(T o);
        bool Delete(ID id);
    }
}
