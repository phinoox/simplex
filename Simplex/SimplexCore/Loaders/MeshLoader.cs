using Simplex.Core.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using glTFLoader;
using System.IO;

namespace Simplex.Core.Loaders
{
    public class MeshLoader
    {

        public  Mesh LoadMesh(string path)
        {
            Mesh mesh = new Mesh();

            if (!File.Exists(path))
                return mesh;
            glTFLoader.Schema.Gltf gltf = Interface.LoadModel(path);
            

            return mesh;
        }

    }
}
