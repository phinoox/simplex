using Simplex.Core;
using Simplex.Core.Gui;
using Simplex.Core.Loaders;
using Simplex.Core.Rendering;
using System.IO;

namespace Simplex.Editor
{
    internal class EditorApplication : ApplicationBase
    {
        #region Private Fields

        private Control ctrl;
        private string guiPath;
        private string scenePath;

        #endregion Private Fields

        #region Private Methods

        private void drawGui()
        {
        }

        private void MainWindow_FileDrop(object sender, OpenTK.Input.FileDropEventArgs e)
        {
            string ext = Path.GetExtension(e.FileName);
            switch (ext)
            {
                case ".gltf":
                    {
                        MeshLoader mloader = new MeshLoader();
                        Mesh mesh = mloader.LoadMesh(e.FileName);
                        break;
                    }
                case ".xaml":
                    {
                        GuiLoader gLoader = new GuiLoader();
                        gLoader.LoadGui(e.FileName);
                        break;
                    }
            }
        }

        #endregion Private Methods

        #region Protected Methods

        protected override void onInit(string[] args)
        {
            base.onInit(args);
            string sponza = "Data\\sponza\\Sponza.gltf";

            //ctrl = new Control();
            MainWindow.FileDrop += MainWindow_FileDrop;
        }

        protected override void onTick(float delta)
        {
            base.onTick(delta);
            if (MainWindow.isKeyDown(OpenTK.Input.Key.Escape))
                this.ShouldClose = true;
            //ctrl.Draw();
        }

        #endregion Protected Methods
    }
}