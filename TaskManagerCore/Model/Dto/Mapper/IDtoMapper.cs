namespace TaskManagerCore.Model.Dto.Mapper
{
    public interface IDtoMapper<T, V>
    {
        public V Map(T entity);
    }
}
