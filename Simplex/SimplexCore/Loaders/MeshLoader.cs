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
        Dictionary<string, Mesh> meshes = new Dictionary<string, Mesh>();

        public  Mesh LoadMesh(string path)
        {
            Mesh mesh = new Mesh();

            if (!File.Exists(path))
                return mesh;
            glTFLoader.Schema.Gltf gltf = Interface.LoadModel(path);

            Dictionary<int, ObjectTK.Textures.Texture2D> textures = new Dictionary<int, ObjectTK.Textures.Texture2D>();

            int textureCount = 0;

            foreach(glTFLoader.Schema.Texture tex in gltf.Textures)
            {
                int? source = tex.Source;
                if (!source.HasValue)
                    continue;
                string fileName = Path.GetDirectoryName(path) + gltf.Images[source.Value].Uri;
                textures[textureCount] = TextureLoader.Instance.LoadTexture2D(fileName);
                textureCount++;
            }

            foreach(glTFLoader.Schema.Material gltfmat in gltf.Materials)
            {
                PbrMaterial mat = new PbrMaterial();
                mat.Metalicness = gltfmat.PbrMetallicRoughness.MetallicFactor;
                mat.Roughness = gltfmat.PbrMetallicRoughness.RoughnessFactor;
                mat.EmissiveFactor = gltfmat.EmissiveFactor[0];
                mat.BasecolorFactor = gltfmat.PbrMetallicRoughness.BaseColorFactor[0];
                
                mat.MetalTexture = textures[gltfmat.PbrMetallicRoughness.MetallicRoughnessTexture.Index];
                mat.Albedo = textures[gltfmat.PbrMetallicRoughness.BaseColorTexture.Index];
                mat.EmissiveMap = textures[gltfmat.EmissiveTexture.Index];
                mat.NormalMap = textures[gltfmat.NormalTexture.Index];
                mat.SsaoMap = textures[gltfmat.OcclusionTexture.Index];

                mesh.Materials.Add(mat);
            }

            mesh.Vbo = new ObjectTK.Buffers.VertexArray();
            

            return mesh;
        }

    }
}
