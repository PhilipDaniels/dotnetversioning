namespace SetVersion.Lib
{
    /// <summary>
    /// Represents a service which can generate a new version number based on a pattern
    /// and a current version (which can be unspecified).
    /// </summary>
    public interface IPatternApplier
    {
        /// <summary>
        /// Generate a new version number based on a pattern and an existing verison number.
        /// </summary>
        /// <param name="versionPattern">The version pattern.</param>
        /// <param name="currentVersion">The current version. Used as the basis for any <c>{{Inc}}</c> variables.</param>
        /// <returns>A new version number.</returns>
        string GetNewVersion(string versionPattern, string currentVersion);
    }
}
