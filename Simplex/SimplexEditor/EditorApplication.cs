using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Simplex.Core;
using Simplex.Core.Gui;
using Simplex.Core.Loaders;
using Simplex.Core.Rendering;

namespace Simplex.Editor
{
    class EditorApplication : ApplicationBase
    {
        Control ctrl;
        string scenePath;
        string guiPath;
        protected override void onInit(string[] args)
        {
            base.onInit(args);
            string sponza = "Data\\sponza\\Sponza.gltf";
            
            //ctrl = new Control();
            MainWindow.FileDrop += MainWindow_FileDrop;
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

        private void drawGui()
        {
           
        }

        protected override void onTick(float delta)
        {
            base.onTick(delta);
            if (MainWindow.isKeyDown(OpenTK.Input.Key.Escape))
                this.ShouldClose = true;
            //ctrl.Draw();
        }
    }
}
