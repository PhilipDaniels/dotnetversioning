using System;

namespace SetVersion.Lib
{
    /// <summary>
    /// Provides utility methods for validating arguments to methods.
    /// </summary>
    static class Val
    {
        /// <summary>
        /// Throws an <code>ArgumentNullException</code> if <paramref name="parameter"/> is null.
        /// </summary>
        /// <typeparam name="T">Generic type of the argument.</typeparam>
        /// <param name="parameter">The parameter itself.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns><paramref name="parameter"/> if no exception is thrown.</returns>
        public static T ThrowIfNull<T>([ValidatedNotNull] T parameter, string parameterName)
        {
            return ThrowIfNull(parameter, parameterName, null);
        }

        /// <summary>
        /// Throws an <code>ArgumentNullException</code> if <paramref name="parameter"/> is null.
        /// </summary>
        /// <typeparam name="T">Generic type of the argument.</typeparam>
        /// <param name="parameter">The parameter itself.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="message">Message to associate with the exception.</param>
        /// <returns><paramref name="parameter"/> if no exception is thrown.</returns>
        public static T ThrowIfNull<T>([ValidatedNotNull] T parameter, string parameterName, string message)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(parameterName, message);
            }

            return parameter;
        }
    }
}
