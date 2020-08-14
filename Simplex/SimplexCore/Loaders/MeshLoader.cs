using glTFLoader;
using Simplex.Core.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GLTF = glTFLoader.Schema;



public class GltfMeshData<T> : MeshData where T : unmanaged
{
    List<T> _data = new List<T>();
    GLTF.Accessor.TypeEnum _dataType;
    GLTF.Accessor.ComponentTypeEnum _componentType;


    int _count;

    public GltfMeshData(GLTF.Accessor.TypeEnum dataType, GLTF.Accessor.ComponentTypeEnum componentType, byte[] rawData, int count)
    {
        this._dataType = dataType;
        this._componentType = componentType;
        this._count = count;

        readData(rawData);

    }


    private unsafe void readData(byte[] rawData)
    {
        int offset = sizeof(T);
        int size = rawData.Length / offset;

        fixed (byte* start = &rawData[0])
        {
            T* first = (T*)start;
            for (T* tp = (T*)start; tp < (T*)start + size; tp++)
            {
                _data.Add(*tp);
            }
        }

    }



}




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
            string dirName = Path.GetDirectoryName(path) + Path.DirectorySeparatorChar;
            if (!File.Exists(path))
                return mesh;
            GLTF.Gltf gltf = Interface.LoadModel(path);

            Dictionary<int, ObjectTK.Textures.Texture2D> textures = new Dictionary<int, ObjectTK.Textures.Texture2D>();

            int textureCount = 0;

            if (gltf.Textures != null)
            {
                foreach (GLTF.Texture tex in gltf.Textures)
                {
                    int? source = tex.Source;
                    if (!source.HasValue)
                        continue;
                    string fileName = dirName + gltf.Images[source.Value].Uri;
                    textures[textureCount] = TextureLoader.Instance.LoadTexture2D(fileName);
                    textureCount++;
                }
            }

            List<PbrMaterial> materials = new List<PbrMaterial>();

            if (gltf.Materials != null)
            {
                foreach (GLTF.Material gltfmat in gltf.Materials)
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

                    materials.Add(mat);
                }
            }

            mesh.Vbo = new ObjectTK.Buffers.VertexArray();
            List<Byte[]> buffers = new List<byte[]>();
            foreach (GLTF.Buffer buffer in gltf.Buffers)
            {
                const string dataDesc = "data:application/octet-stream;base64,";
                if (buffer.Uri.StartsWith(dataDesc))
                {
                    string bufferData = buffer.Uri.Substring(dataDesc.Length);
                    Byte[] bytes = Convert.FromBase64String(bufferData);
                    buffers.Add(bytes);
                }
                else
                {
                    string bufferPath = dirName + buffer.Uri;
                    if (!File.Exists(bufferPath))
                    {
                        Console.WriteLine($"could not find file {bufferPath}");
                        continue;
                    }
                    Byte[] bytes = File.ReadAllBytes(bufferPath);
                    buffers.Add(bytes);
                }
            }

            List<byte[]> bufferViews = new List<byte[]>();

            foreach (GLTF.BufferView bv in gltf.BufferViews)
            {
                byte[] bva = new byte[bv.ByteLength];
                Array.Copy(buffers[bv.Buffer], bv.ByteOffset, bva, 0, bv.ByteLength);
                bufferViews.Add(bva);
            }

            List<object> accessors = new List<object>();

            List<MeshData> meshDatas = new List<MeshData>();
            foreach (GLTF.Accessor acci in gltf.Accessors)
            {
                MeshData data;
                switch (acci.ComponentType)
                {
                    case GLTF.Accessor.ComponentTypeEnum.UNSIGNED_INT:
                        data = new GltfMeshData<uint>(acci.Type, acci.ComponentType, bufferViews[acci.BufferView.Value], acci.Count);
                        break;
                    case GLTF.Accessor.ComponentTypeEnum.UNSIGNED_SHORT:
                        data = new GltfMeshData<ushort>(acci.Type, acci.ComponentType, bufferViews[acci.BufferView.Value], acci.Count);
                        break;
                    case GLTF.Accessor.ComponentTypeEnum.UNSIGNED_BYTE:
                        data = new GltfMeshData<byte>(acci.Type, acci.ComponentType, bufferViews[acci.BufferView.Value], acci.Count);
                        break;
                    case GLTF.Accessor.ComponentTypeEnum.SHORT:
                        data = new GltfMeshData<short>(acci.Type, acci.ComponentType, bufferViews[acci.BufferView.Value], acci.Count);
                        break;
                    case GLTF.Accessor.ComponentTypeEnum.FLOAT:
                        data = new GltfMeshData<float>(acci.Type, acci.ComponentType, bufferViews[acci.BufferView.Value], acci.Count);
                        break;
                    case GLTF.Accessor.ComponentTypeEnum.BYTE:
                    default:
                        data = new GltfMeshData<sbyte>(acci.Type, acci.ComponentType, bufferViews[acci.BufferView.Value], acci.Count);
                        break;

                }
                meshDatas.Add(data);
            }

            foreach (GLTF.Mesh gltfMesh in gltf.Meshes)
            {

                foreach (GLTF.MeshPrimitive gltfPrimitive in gltfMesh.Primitives)
                {
                    MeshPrimitive primitive = new MeshPrimitive();
                    meshDatas[gltfPrimitive.Indices.Value].AttributeName = "INDICES";
                    primitive.MeshDatas.Add(meshDatas[gltfPrimitive.Indices.Value]);
                    if (gltfPrimitive.Material.HasValue)
                        primitive.Material = materials[gltfPrimitive.Material.Value];
                    foreach (KeyValuePair<string, int> attribute in gltfPrimitive.Attributes)
                    {
                        meshDatas[attribute.Value].AttributeName = attribute.Key;
                        primitive.MeshDatas.Add(meshDatas[attribute.Value]);
                    }
                    mesh.Primitives.Add(primitive);
                }
            }


            return mesh;
        }

        #endregion Public Methods
    }
}