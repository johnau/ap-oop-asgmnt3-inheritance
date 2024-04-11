using TaskManagerCore.Configuration;

namespace TaskManagerCore.Model.Repository
{
    /// <summary>
    /// Interface for the repository managing task folder entities.
    /// </summary>
    public interface ITaskFolderRepository : ICrudRepository<TaskFolder, string>
    {
        /// <summary>
        /// Finds a task folder by name.
        /// </summary>
        /// <param name="name">The name of the task folder to search for.</param>
        /// <returns>The task folder entity matching the specified name, or null if not found.</returns>
        TaskFolder? FindByName(string name);

        /// <summary>
        /// Finds task folders whose names start with the specified string.
        /// </summary>
        /// <param name="name">The string to match at the beginning of task folder names.</param>
        /// <returns>A list of task folder entities whose names start with the specified string.</returns>
        List<TaskFolder> FindByNameStartsWith(string name);

        /// <summary>
        /// Finds a single task folder by name.
        /// </summary>
        /// <param name="name">The name of the task folder to search for.</param>
        /// <returns>The first task folder entity found with the specified name, or null if not found.</returns>
        TaskFolder? FindOneByName(string name);

        /// <summary>
        /// Finds task folders that are empty.
        /// </summary>
        /// <returns>A list of task folder entities that are empty.</returns>
        List<TaskFolder> FindEmpty();

        /// <summary>
        /// Finds task folders that are not empty.
        /// </summary>
        /// <returns>A list of task folder entities that are not empty.</returns>
        List<TaskFolder> FindNotEmpty();

        /// <summary>
        /// Deletes a task folder by name.
        /// </summary>
        /// <param name="name">The name of the task folder to delete.</param>
        /// <returns>True if the task folder was successfully deleted, otherwise false.</returns>
        bool DeleteByName(string name);
    }
}
