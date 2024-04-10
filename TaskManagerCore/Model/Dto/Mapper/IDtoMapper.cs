namespace TaskManagerCore.Model.Dto.Mapper
{
    internal interface IDtoMapper<T, V>
    {
        public V Map(T entity);
    }
}
