using System;

namespace Car.Components
{
    /// <summary>
    /// The engine.
    /// </summary>
    public class Engine
    {
        /// <summary>
        /// Runs this instance.
        /// </summary>
        public string Run()
        {
            return DateTime.Now.ToString();
        }
    }
}
