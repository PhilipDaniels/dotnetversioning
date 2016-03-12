using System;

namespace SetVersion.Lib
{
    /// <summary>
    /// VS Code Analysis will not actually recognize the generic helper method
    /// (and possibly sometimes non-generics) as having checked parameters, so
    /// CA1062 warnings will not go away. By introducing this dummy attribute
    /// we can fool the code analysis engine, and stop it producing false
    /// warnings.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    sealed class ValidatedNotNullAttribute : Attribute
    {
    }
}
