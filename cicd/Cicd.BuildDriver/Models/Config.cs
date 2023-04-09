using System.Collections.Generic;

namespace RecitalBlooms.Website.Cicd.Utils
{
    /// <summary>
    /// Represents a template deployment config file.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Gets or sets the config file tags.
        /// </summary>
        public Dictionary<string, string> Tags { get; set; } = new();

        /// <summary>
        /// Gets or sets the config file parameters.
        /// </summary>
        public Dictionary<string, Parameter> Parameters { get; set; } = new();
    }
}
