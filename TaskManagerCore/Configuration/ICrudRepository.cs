using TaskManagerCore.Model.Repository;

namespace TaskManagerCore.Configuration
{
    /// <summary>
    /// Generic CRUD Repository Interface
    /// </summary>
    /// <typeparam name="T">Type of the repository object</typeparam>
    /// <typeparam name="ID">Type of the repository object's ID</typeparam>
    public interface ICrudRepository<T, ID>
        where T : class
    {
        /// <summary>
        /// Retrieves all objects stored in the repository.
        /// </summary>
        /// <returns>A list of <typeparamref name="T"/> objects, or an empty list if no objects are found.</returns>
        List<T> FindAll();

        /// <summary>
        /// Retrieves all objects matching the provided IDs from the repository.
        /// </summary>
        /// <param name="ids">A list of IDs to match.</param>
        /// <returns>A list of <typeparamref name="T"/> objects, or an empty list if no matching objects are found.</returns>
        List<T> FindByIds(List<ID> ids);

        /// <summary>
        /// Retrieves an object by its ID.
        /// </summary>
        /// <param name="id">The ID of the object to retrieve.</param>
        /// <returns>
        /// The <typeparamref name="T"/> object found with the specified ID, or <c>null</c> if no matching object is found.
        /// </returns>
        T? FindById(ID id);

        /// <summary>
        /// Saves the provided object and returns its ID.
        /// </summary>
        /// <param name="o">The object to save.</param>
        /// <returns>The ID of the saved object.</returns>
        ID Save(T o);

        /// <summary>
        /// Deletes the object with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the object to delete.</param>
        /// <returns><c>true</c> if the object was successfully deleted; otherwise, <c>false</c>.</returns>
        bool Delete(ID id);
    }
}
