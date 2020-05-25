namespace Simplex.Core.Configs
{
    /// <summary>
    /// Base configuration for the application
    /// </summary>
    public sealed class ApplicationConfig
    {
        #region Private Fields

        private string DataDirectory = "Data";
        private float fps = 30;
        private string title = "Simplex Application";

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// the path to the data/assets
        /// </summary>
        public string DataDirectory1 { get => DataDirectory; set => DataDirectory = value; }

        /// <summary>
        /// the fps limit
        /// </summary>
        public float Fps { get => fps; set => fps = value; }

        /// <summary>
        /// the title of the application
        /// </summary>
        public string Title { get => title; set => title = value; }

        #endregion Public Properties
    }
}