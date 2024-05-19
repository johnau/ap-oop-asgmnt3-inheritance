using System;

namespace TaskManagerCore.Model.Dto
{
    /// <summary>
    /// Data Transfer Object (DTO) representing a new folder.
    /// <para>
    /// This DTO layer serves as a boundary between the business layers and the view layers.
    /// The view layer is able to rely on this data structure despite potential changes to
    /// the underlying model. Conversly, the data exposed to the view layer can be altered
    /// without making changes to the model, ie. to conceal the database id which does not 
    /// need to be exposed to the view layer.
    /// </para>
    /// </summary>
    public class CreateFolderDto
    {
        /// <value>
        /// The <c>Name</c> for the new folder
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateFolderDto"/> class.
        /// </summary>
        /// <param name="name">The name of the folder.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <see langword="null"/>.</exception>
        public CreateFolderDto(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name), "Name cannot be null.");
        }
    }
}