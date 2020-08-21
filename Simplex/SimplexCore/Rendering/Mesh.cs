using ObjectTK.Buffers;
using OpenTK;
using System.Collections.Generic;
using Simplex.Core;
using System;
using OpenTK.Graphics.OpenGL;

namespace Simplex.Core.Rendering
{

    public class MeshPrimitive
    {
        private bool initialized = false;
        private PbrMaterial _material;
        private Buffer<Vector3> _vbo;
        private Buffer<Vector2> _tex;
        private Buffer<uint> _indexBuffer;

        private Buffer<Vector3> _normals;

        PrimitiveType _drawMode = PrimitiveType.Triangles;
        private VertexArray _vao;
        private List<MeshData> meshDatas = new List<MeshData>();

        private BoundingBox _bounds = new BoundingBox();

        ~MeshPrimitive()
        {
            _vao.Dispose();
            _vbo.Dispose();
            _indexBuffer.Dispose();
            _material.Dispose();
        }

        private void CreateVBO(MeshData data)
        {
            _vbo = new Buffer<Vector3>();
            List<Vector3> vertices = new List<Vector3>();
            Vector3 minBounds = new Vector3();
            Vector3 maxBounds = new Vector3();
            bool first = true;
            foreach (object obj in data.Data)
            {
                try
                {
                    Vector3 vec3 = (Vector3)obj;
                    if (first)
                    {
                        minBounds = vec3;
                        maxBounds = vec3;
                        first = false;
                    }
                    if (vec3.X < minBounds.X)
                        minBounds.X = vec3.X;
                    else if (vec3.X > maxBounds.X)
                        maxBounds.X = vec3.X;

                    if (vec3.Y < minBounds.Y)
                        minBounds.Y = vec3.Y;
                    else if (vec3.Y > maxBounds.Y)
                        maxBounds.Y = vec3.Y;

                    if (vec3.Z < minBounds.Z)
                        minBounds.Z = vec3.Z;
                    else if (vec3.Z > maxBounds.Z)
                        maxBounds.Z = vec3.Z;
                    vertices.Add(vec3);
                }
                catch (Exception e)
                {
                    throw (e);
                }
            }
            _bounds.Min=minBounds;
            _bounds.Max=maxBounds;
            
            _vbo.Init(BufferTarget.ArrayBuffer, vertices.ToArray());
        }
        private void CreateNormalBuffer(MeshData data)
        {
            _normals = new Buffer<Vector3>();
            List<Vector3> vertices = new List<Vector3>();
            foreach (object obj in data.Data)
            {
                try
                {
                    Vector3 vec3 = (Vector3)obj;
                    vertices.Add(vec3);
                }
                catch (Exception e)
                {
                    throw (e);
                }
            }

            _normals.Init(BufferTarget.ArrayBuffer, vertices.ToArray());
        }

        private void createIndexBuffer(MeshData data)
        {
            _indexBuffer = new Buffer<uint>();
            List<uint> indices = new List<uint>();
            foreach (object obj in data.Data)
            {
                try
                {
                    uint indice = Convert.ToUInt32(obj);
                    indices.Add(indice);
                }
                catch (Exception e)
                {
                    throw (e);
                }
            }
            _indexBuffer.Init(BufferTarget.ElementArrayBuffer, indices.ToArray());
        }

        private void CreateTexCoordBuffer(MeshData data)
        {
            _tex = new Buffer<Vector2>();
            List<Vector2> coords = new List<Vector2>();
            foreach (object obj in data.Data)
            {
                try
                {
                    Vector2 vec2 = (Vector2)obj;
                    coords.Add(vec2);
                }
                catch (Exception e)
                {
                    throw (e);
                }
            }

            _tex.Init(BufferTarget.ArrayBuffer, coords.ToArray());
        }

        public List<MeshData> MeshDatas { get => meshDatas; }
        public PbrMaterial Material { get => _material; set => _material = value; }
        public PrimitiveType DrawMode { get => _drawMode; set => _drawMode = value; }

        public void Init()
        {
            if (initialized)
                return;
            _vao = new VertexArray();
            _vao.Bind();
            _material.Init();

            foreach (MeshData meshData in meshDatas)
            {
                //if(meshData.Initialized)
                switch (meshData.AttributeName)
                {
                    case "POSITION": CreateVBO(meshData); break;
                    case "INDICES": createIndexBuffer(meshData); break;
                    case "TEXCOORD_0": CreateTexCoordBuffer(meshData); break;
                    case "NORMAL": CreateNormalBuffer(meshData); break;
                }
            }

            InitVao();
            _bounds.Init();
            initialized = true;
        }


        private void InitVao()
        {

            // set up binding of the shader variable to the buffer object
            _vao.BindAttribute(_material.ShaderProgram.InPosition, _vbo);
            if (_tex != null)
                _vao.BindAttribute(_material.ShaderProgram.InTexCoord, _tex);
            if (_indexBuffer != null)
                _vao.BindElementBuffer(_indexBuffer);
            if (_normals != null)
                _vao.BindAttribute(_material.ShaderProgram.InNormal, _normals);
        }
        public void Render(in Matrix4 mvp)
        {
            _material.Use();

            _material.ShaderProgram.ModelViewProjectionMatrix.Set(mvp);
            _material.ShaderProgram.FragColor.Set(new OpenTK.Color(255, 0, 0, 1));
            _material.ShaderProgram.LightColor.Set(GlobalUniforms.LightColor);
            _material.ShaderProgram.LightDir.Set(GlobalUniforms.LightDir);
            _material.ShaderProgram.Ambient.Set(GlobalUniforms.AmbientColor);
            _vao.Bind();
            if (_indexBuffer == null)
                _vao.DrawArrays(_drawMode, 0, _vbo.ElementCount);
            else
            {
                _vao.BindElementBuffer(_indexBuffer);
                _vao.DrawElements(_drawMode, _indexBuffer.ElementCount);
            }

        }
    }

    /// <summary>
    /// base class for mesh information
    /// </summary>
    public class Mesh
    {
        #region Private Fields

        private List<MeshPrimitive> primitives = new List<MeshPrimitive>();



        #endregion Private Fields

        #region Public Properties
        public List<MeshPrimitive> Primitives { get => primitives; set => primitives = value; }


        #endregion Public Properties

        #region Public Methods

        public void Render(in Matrix4 mvp)
        {

            foreach (MeshPrimitive primitive in Primitives)
            {
                primitive.Render(mvp);
            }
        }

        public void Init()
        {
            foreach (MeshPrimitive primitive in primitives)
            {
                primitive.Init();
            }
        }

        #endregion Public Methods
    }
}