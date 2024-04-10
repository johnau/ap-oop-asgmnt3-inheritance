namespace TaskManagerCore.Model.Dto
{
    /// <summary>
    /// Data Transfer Object (DTO) representing a new folder.
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