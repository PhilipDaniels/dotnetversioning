namespace SetVersion.Lib
{
    /// <summary>
    /// An interface for classes that can interrogate VCS systems, to discover
    /// commit Ids and branch names.
    /// </summary>
    public interface IVCSInterrogator
    {
        /// <summary>
        /// Gets information from a VCS for a specific working directory.
        /// </summary>
        /// <returns>Appropriate <seealso cref="VCSInfo"/>, will not be null, but the properties
        /// of it might be.</returns>
        VCSInfo GetInfo();
    }
}
