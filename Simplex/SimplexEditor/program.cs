using Simplex.Editor;
using System;

namespace SimplexTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            EditorApplication app = new EditorApplication();
            app.Init(args);
            app.Run();
        }
    }
}
