using System.Collections.Generic;

namespace TaskManagerCore.Model.Dto
{
    /// <summary>
    /// Data Transfer Object (DTO) representing a folder entity retrieved from the system.
    /// <para>
    /// This DTO layer serves as a boundary between the business layers and the view layers.
    /// The view layer is able to rely on this data structure despite potential changes to
    /// the underlying model. Conversly, the data exposed to the view layer can be altered
    /// without making changes to the model, ie. to conceal the database id which does not 
    /// need to be exposed to the view layer.
    /// </para>
    /// </summary>
    public class GetFolderDto
    {
        /// <value>
        /// The unique identifier of the folder.
        /// </value>
        public string Id { get; }

        /// <value>
        /// The unique name of the folder.
        /// </value>
        public string Name { get; }

        /// <value>
        /// The list of unique identifiers of tasks associated with the folder.
        /// </value>
        public List<string> TaskIds { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetFolderDto"/> class with the specified parameters.
        /// </summary>
        /// <param name="id">The unique identifier of the folder.</param>
        /// <param name="name">The unique name of the folder.</param>
        /// <param name="taskIds">The list of unique identifiers of tasks associated with the folder.</param>
        public GetFolderDto(string id, string name, List<string> taskIds)
        {
            Id = id;
            Name = name;
            TaskIds = taskIds;
        }
    }
}