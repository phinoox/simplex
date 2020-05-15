using System;
using System.Collections.Generic;
using System.Text;
using Simplex.Core;
using Simplex.Core.Loaders;
using Simplex.Core.Rendering;

namespace Simplex.Editor
{
    class EditorApplication : ApplicationBase
    {

        protected override void onInit(string[] args)
        {
            base.onInit(args);
            string sponza = "Data\\sponza\\Sponza.gltf";
            MeshLoader mloader = new MeshLoader();
            Mesh mesh = mloader.LoadMesh(sponza);
        }

        protected override void onTick(float delta)
        {
            base.onTick(delta);
            if (MainWindow.isKeyDown(OpenTK.Input.Key.Escape))
                this.ShouldClose = true;
        }
    }
}
