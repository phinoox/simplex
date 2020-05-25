using glTFLoader;
using Simplex.Core.Rendering;
using System;
using System.Collections.Generic;
using System.IO;

namespace Simplex.Core.Loaders
{
    /// <summary>
    /// class for loading gltf models
    /// </summary>
    public class MeshLoader
    {
        #region Private Fields

        private Dictionary<string, Mesh> meshes = new Dictionary<string, Mesh>();

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// loads a mesh from a gltf file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Mesh LoadMesh(string path)
        {
            Mesh mesh = new Mesh();

            if (!File.Exists(path))
                return mesh;
            glTFLoader.Schema.Gltf gltf = Interface.LoadModel(path);

            Dictionary<int, ObjectTK.Textures.Texture2D> textures = new Dictionary<int, ObjectTK.Textures.Texture2D>();

            int textureCount = 0;

            foreach (glTFLoader.Schema.Texture tex in gltf.Textures)
            {
                int? source = tex.Source;
                if (!source.HasValue)
                    continue;
                string fileName = Path.GetDirectoryName(path) + gltf.Images[source.Value].Uri;
                textures[textureCount] = TextureLoader.Instance.LoadTexture2D(fileName);
                textureCount++;
            }

            foreach (glTFLoader.Schema.Material gltfmat in gltf.Materials)
            {
                PbrMaterial mat = new PbrMaterial();
                mat.Metalicness = gltfmat.PbrMetallicRoughness.MetallicFactor;
                mat.Roughness = gltfmat.PbrMetallicRoughness.RoughnessFactor;
                mat.EmissiveFactor = gltfmat.EmissiveFactor[0];
                mat.BasecolorFactor = gltfmat.PbrMetallicRoughness.BaseColorFactor[0];
                if (gltfmat.PbrMetallicRoughness.MetallicRoughnessTexture != null)
                    mat.MetalTexture = textures[gltfmat.PbrMetallicRoughness.MetallicRoughnessTexture.Index];
                if (gltfmat.PbrMetallicRoughness.BaseColorTexture != null)
                    mat.Albedo = textures[gltfmat.PbrMetallicRoughness.BaseColorTexture.Index];
                if (gltfmat.EmissiveTexture != null)
                    mat.EmissiveMap = textures[gltfmat.EmissiveTexture.Index];
                if (gltfmat.NormalTexture != null)
                    mat.NormalMap = textures[gltfmat.NormalTexture.Index];
                if (gltfmat.OcclusionTexture != null)
                    mat.SsaoMap = textures[gltfmat.OcclusionTexture.Index];

                mesh.Materials.Add(mat);
            }

            mesh.Vbo = new ObjectTK.Buffers.VertexArray();
            List<Byte[]> buffers = new List<byte[]>();
            foreach (glTFLoader.Schema.Buffer buffer in gltf.Buffers)
            {
                if (!File.Exists(buffer.Uri))
                    continue;
                Byte[] bytes = File.ReadAllBytes(buffer.Uri);
                buffers.Add(bytes);
            }

            return mesh;
        }

        #endregion Public Methods
    }
}