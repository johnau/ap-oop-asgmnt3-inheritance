namespace TaskManagerCore.Model.Dto.Mapper
{
    internal interface IDtoMapper<T, V>
    {
        V Map(T entity);
    }
}
