using Simplex.Editor;

namespace SimplexTestApp
{
    internal class Program
    {
        #region Private Methods

        private static void Main(string[] args)
        {
            EditorApplication app = new EditorApplication();
            app.Init(args);
            app.Run();
        }

        #endregion Private Methods
    }
}