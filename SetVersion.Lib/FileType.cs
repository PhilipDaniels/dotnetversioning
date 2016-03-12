namespace SetVersion.Lib
{
    /// <summary>
    /// Represents the different types of files the program understands.
    /// </summary>
    public enum FileType
    {
        /// <summary>
        /// A JSON file, typically a project.json file.
        /// </summary>
        Json,

        /// <summary>
        /// A C# file.
        /// </summary>
        CSharp,

        /// <summary>
        /// A Visual Basic file.
        /// </summary>
        VisualBasic,

        /// <summary>
        /// An F# file.
        /// </summary>
        FSharp,

        /// <summary>
        /// An assembly (dll or exe).
        /// </summary>
        Assembly
    }
}
