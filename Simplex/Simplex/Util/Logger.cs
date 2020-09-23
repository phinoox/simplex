using System.Reflection;

namespace Simplex.Util
{
    /// <summary>
    /// wrapper for log4net
    /// </summary>
    public static class Logger
    {
        #region Public Fields

        /// <summary>
        /// the logger for default logs
        /// </summary>
        public static readonly log4net.ILog Default = log4net.LogManager.GetLogger(Assembly.GetExecutingAssembly(), "Simplex");

        #endregion Public Fields
    }
}