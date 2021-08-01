using glTFLoader;
using OpenTK;
using OpenTK.Mathematics;
using Simplex.Core.Components;
using Simplex.Core.Rendering;
using Simplex.Core.Scene;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GLTF = glTFLoader.Schema;





namespace Simplex.Core.Loaders
{


    /// <summary>
    /// class for loading gltf models
    /// </summary>
    public class MeshLoader
    {
        #region Private Fields

        private Dictionary<string, MeshComponent> meshes = new Dictionary<string, MeshComponent>();

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// loads a mesh from a gltf file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public SceneNode LoadMesh(string path)
        {
            SceneNode rootNode = new SceneNode();
            string dirName = Path.GetDirectoryName(path) + Path.DirectorySeparatorChar;
            if (!File.Exists(path))
                return rootNode;
            GLTF.Gltf gltf = Interface.LoadModel(path);
             
            Dictionary<int, ObjectTK.Textures.Texture2D> textures = new Dictionary<int, ObjectTK.Textures.Texture2D>();

            int textureCount = 0;

            if (gltf.Textures != null)
            {
                foreach (GLTF.Texture tex in gltf.Textures)
                {
                    int? source = tex.Source;
                    if (!source.HasValue){
                       continue;
                    }
                    if(gltf.Images[source.Value].Uri==null){
                        Stream imageStream = Interface.OpenImageFile(gltf,textureCount,path);
                        textures[textureCount] = TextureLoader.Instance.LoadTexture2D(imageStream,path+$"_{textureCount}");
                        textureCount++;
                        continue;   
                    }
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
                    switch (gltfmat.AlphaMode)
                    {
                        case GLTF.Material.AlphaModeEnum.OPAQUE: mat.Blending = AlphaMode.OPAQUE;break;
                        case GLTF.Material.AlphaModeEnum.MASK: mat.Blending = AlphaMode.MASKED; break;
                        case GLTF.Material.AlphaModeEnum.BLEND: mat.Blending = AlphaMode.BLEND; break;
                    }
                    mat.Blending = gltfmat.AlphaMode == GLTF.Material.AlphaModeEnum.OPAQUE ? AlphaMode.OPAQUE : AlphaMode.MASKED;
                    mat.DoubleSided = gltfmat.DoubleSided;
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


            List<Byte[]> buffers = new List<byte[]>();
            foreach (GLTF.Buffer buffer in gltf.Buffers)
            {
                const string dataDesc = "data:application/octet-stream;base64,";
                if(buffer.Uri==null){
                    Byte[] bytes = Interface.LoadBinaryBuffer(path);
                    buffers.Add(bytes);
                }
                else if (buffer.Uri.StartsWith(dataDesc))
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
                MeshData data = new MeshData();
                ArrayList originTypedData = new ArrayList();

                if (acci.ComponentType == GLTF.Accessor.ComponentTypeEnum.FLOAT)
                {
                    List<float> fdata = readFloatData<float>(bufferViews[acci.BufferView.Value]);
                    switch (acci.Type)
                    {
                        case GLTF.Accessor.TypeEnum.VEC2:
                            for (int i = 0; i < fdata.Count; i += 2)
                            {
                                Vector2 vec2 = new Vector2(fdata[i], fdata[i + 1]);
                                originTypedData.Add(vec2);
                            }
                            break;
                        case GLTF.Accessor.TypeEnum.VEC3:
                            for (int i = 0; i < fdata.Count; i += 3)
                            {
                                Vector3 vec3 = new Vector3(fdata[i], fdata[i + 1], fdata[i + 2]);
                                originTypedData.Add(vec3);
                            }
                            break;
                        case GLTF.Accessor.TypeEnum.VEC4:
                            for (int i = 0; i < fdata.Count; i += 4)
                            {
                                Vector4 vec4 = new Vector4(fdata[i], fdata[i + 1], fdata[i + 2], fdata[i + 3]);
                                originTypedData.Add(vec4);
                            }
                            break;
                        default:
                            originTypedData.AddRange(fdata);

                            break;
                    }
                }
                else
                {

                    List<int> idata = new List<int>();
                    switch (acci.ComponentType)
                    {

                        case GLTF.Accessor.ComponentTypeEnum.UNSIGNED_INT:
                            idata.AddRange(readIntData<uint>(bufferViews[acci.BufferView.Value]));
                            break;
                        case GLTF.Accessor.ComponentTypeEnum.UNSIGNED_SHORT:
                            idata.AddRange(readIntData<ushort>(bufferViews[acci.BufferView.Value]));
                            break;
                        case GLTF.Accessor.ComponentTypeEnum.UNSIGNED_BYTE:
                            idata.AddRange(readIntData<byte>(bufferViews[acci.BufferView.Value]));
                            break;
                        case GLTF.Accessor.ComponentTypeEnum.SHORT:
                            idata.AddRange(readIntData<short>(bufferViews[acci.BufferView.Value]));
                            break;
                        case GLTF.Accessor.ComponentTypeEnum.BYTE:
                        default:
                            idata.AddRange(readIntData<sbyte>(bufferViews[acci.BufferView.Value]));
                            break;

                    }

                    switch (acci.Type)
                    {
                        case GLTF.Accessor.TypeEnum.VEC2:
                            for (int i = 0; i < idata.Count; i += 2)
                            {
                                Vector2 vec2 = new Vector2(idata[i], idata[i + 1]);
                                originTypedData.Add(vec2);
                            }
                            break;
                        case GLTF.Accessor.TypeEnum.VEC3:
                            for (int i = 0; i < idata.Count; i += 3)
                            {
                                Vector3 vec3 = new Vector3(idata[i], idata[i + 1], idata[i + 2]);
                                originTypedData.Add(vec3);
                            }
                            break;
                        case GLTF.Accessor.TypeEnum.VEC4:
                            for (int i = 0; i < idata.Count; i += 4)
                            {
                                Vector4 vec4 = new Vector4(idata[i], idata[i + 1], idata[i + 2], idata[i + 3]);
                                originTypedData.Add(vec4);
                            }
                            break;
                        default:
                            originTypedData.AddRange(idata);

                            break;
                    }

                }
                data.Data = originTypedData;

                meshDatas.Add(data);
            }

            List<MeshComponent> meshes = new List<MeshComponent>();
            foreach (GLTF.Mesh gltfMesh in gltf.Meshes)
            {
                MeshComponent mesh = new MeshComponent(null);
                foreach (GLTF.MeshPrimitive gltfPrimitive in gltfMesh.Primitives)
                {
                    MeshPrimitive primitive = new MeshPrimitive();
                    if (gltfPrimitive.Indices != null)
                    {
                        meshDatas[gltfPrimitive.Indices.Value].AttributeName = "INDICES";
                        primitive.MeshDatas.Add(meshDatas[gltfPrimitive.Indices.Value]);
                    }
                    //if(gltfPrimitive.Mode == GLTF.MeshPrimitive.ModeEnum.TRIANGLES)


                    if (gltfPrimitive.Material.HasValue)
                        primitive.Material = materials[gltfPrimitive.Material.Value];
                    else
                        primitive.Material = new PbrMaterial();
                    foreach (KeyValuePair<string, int> attribute in gltfPrimitive.Attributes)
                    {
                        meshDatas[attribute.Value].AttributeName = attribute.Key;
                        primitive.MeshDatas.Add(meshDatas[attribute.Value]);
                    }
                    mesh.Primitives.Add(primitive);
                }
                meshes.Add(mesh);
            }
            HashSet<int> joints = new HashSet<int>();
            if (gltf.Skins != null)
            {
                foreach (GLTF.Skin skin in gltf.Skins)
                {
                    foreach (int jointIndex in skin.Joints)
                    {
                        joints.Add(jointIndex);
                    }
                }
            }

            List<SceneNode> nodes = new List<SceneNode>();

            foreach (GLTF.Scene scene in gltf.Scenes)
            {
                foreach (int nodeIndex in scene.Nodes)
                {
                    SceneNode sceneNode;
                    GLTF.Node node = gltf.Nodes[nodeIndex];
                    if (node.Mesh != null)
                    {
                        sceneNode = new MeshNode() { Mesh = meshes[node.Mesh.Value] };
                        meshes[node.Mesh.Value].Init();
                        meshes[node.Mesh.Value].Parent = sceneNode;
                    }
                    else if (node.Camera != null)
                    {
                        sceneNode = new Camera();
                    }
                    else if (joints.Contains(nodeIndex))
                    {
                        sceneNode = new SkeletonNode();
                    }
                    else
                    {
                        sceneNode = new SceneNode();
                    }

                    sceneNode.Name = node.Name;
                    sceneNode.Scale = new Vector3(node.Scale[0], node.Scale[1], node.Scale[2]);
                    sceneNode.Translation = new Vector3(node.Translation[0], node.Translation[1], node.Translation[2]);
                    sceneNode.Rotation = new Quaternion(node.Rotation[0], node.Rotation[1], node.Rotation[2], node.Rotation[3]);
                    rootNode.AddChild(sceneNode);

                    if(node.Children==null)
                      continue;
                    foreach (int subnodeIndex in node.Children)
                    {
                        GLTF.Node subNode = gltf.Nodes[subnodeIndex];
                        SceneNode childNode;
                        if (subNode.Mesh != null)
                        {
                            childNode = new MeshNode() { Mesh = meshes[subNode.Mesh.Value] };
                            meshes[subNode.Mesh.Value].Init();

                        }
                        else if (subNode.Camera != null)
                        {
                            childNode = new Camera();
                        }
                        else if (joints.Contains(subnodeIndex))
                        {
                            childNode = new SkeletonNode();
                        }
                        else
                        {
                            childNode = new SceneNode();
                        }
                        childNode.Name = subNode.Name;
                        childNode.Scale = new Vector3(subNode.Scale[0], subNode.Scale[1], subNode.Scale[2]);
                        childNode.Translation = new Vector3(subNode.Translation[0], subNode.Translation[1], subNode.Translation[2]);
                        childNode.Rotation = new Quaternion(subNode.Rotation[0], subNode.Rotation[1], subNode.Rotation[2], subNode.Rotation[3]);
                        sceneNode.AddChild(childNode);
                    }

                }
            }

            /*foreach(GLTF.Node node in gltf.Scenes){
                MeshNode meshNode = new MeshNode();
                meshNode.Scale = new Vector3(node.Scale[0],node.Scale[1],node.Scale[2]);
                node.
            }*/


            //mesh.Scale = new Vector3(gltf.Nodes[0].Scale[0],gltf.Nodes[0].Scale[1],gltf.Nodes[0].Scale[2]);
            //mesh.Init();
            rootNode.CalculateBounds();
            return rootNode;
        }

        private unsafe List<T> readData<T>(byte[] rawData) where T : unmanaged
        {
            int offset = sizeof(T);
            int size = rawData.Length / offset;
            List<T> data = new List<T>();
            fixed (byte* start = &rawData[0])
            {
                T* first = (T*)start;
                for (T* tp = (T*)start; tp < (T*)start + size; tp++)
                {
                    data.Add(*tp);
                }
            }

            return data;

        }

        private unsafe List<int> readIntData<T>(byte[] rawData) where T : unmanaged
        {
            int offset = sizeof(T);
            int size = rawData.Length / offset;
            List<int> data = new List<int>();
            fixed (byte* start = &rawData[0])
            {
                T* first = (T*)start;
                for (T* tp = (T*)start; tp < (T*)start + size; tp++)
                {
                    data.Add(Convert.ToInt32(*tp));
                }
            }

            return data;

        }

        private unsafe List<float> readFloatData<T>(byte[] rawData) where T : unmanaged
        {
            int offset = sizeof(T);
            int size = rawData.Length / offset;
            List<float> data = new List<float>();
            fixed (byte* start = &rawData[0])
            {
                T* first = (T*)start;
                for (T* tp = (T*)start; tp < (T*)start + size; tp++)
                {
                    data.Add(Convert.ToSingle(*tp));
                }
            }

            return data;

        }


        #endregion Public Methods
    }
}