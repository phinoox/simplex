﻿using ObjectTK.Buffers;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Simplex.Core.Rendering;
using Simplex.Core.Rendering.Programs;
using Simplex.Core.Scene;
using Simplex.Core.Util;
using System;
using System.Collections.Generic;

namespace Simplex.Core.Components
{

    public class MeshPrimitive : IDisposable
    {
        private bool initialized = false;
        private bool disposed = false;
        private MaterialBase _material;
        private Buffer<Vector3> _vbo;
        private Buffer<Vector2> _tex;
        private Buffer<uint> _indexBuffer;

        private Buffer<Vector3> _normals;
        private Buffer<Vector4> _tangents;
        PrimitiveType _drawMode = PrimitiveType.Triangles;
        private VertexArray _vao;
        private List<MeshData> meshDatas = new List<MeshData>();

       private BoundingBox _bounds = new BoundingBox();
       

        public List<MeshData> MeshDatas { get => meshDatas; }
        public MaterialBase Material { get => _material; set => _material = value; }
        public PrimitiveType DrawMode { get => _drawMode; set => _drawMode = value; }
        public BoundingBox Bounds { get => _bounds; set => _bounds = value; }
       

        ~MeshPrimitive()
        {
            if (!disposed)
                Dispose();
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
            _bounds.Min = minBounds;
            _bounds.Max = maxBounds;

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

        private void CreateTangentBuffer(MeshData data)
        {
            _tangents = new Buffer<Vector4>();
            List<Vector4> vertices = new List<Vector4>();
            foreach (object obj in data.Data)
            {
                try
                {
                    Vector4 vec3 = (Vector4)obj;
                    vertices.Add(vec3);
                }
                catch (Exception e)
                {
                    throw (e);
                }
            }

            _tangents.Init(BufferTarget.ArrayBuffer, vertices.ToArray());
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
                    case "TANGENT": CreateTangentBuffer(meshData); break;
                }
            }

            InitVao();
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
             if (_tangents != null)
                _vao.BindAttribute(_material.ShaderProgram.InTangent, _tangents);
        }


        public void Render(in Matrix4 model, AlphaMode mode)
        {
            _material.Use();

            
            _material.ShaderProgram.View.Set(GlobalUniforms.View);
            _material.ShaderProgram.Projection.Set(GlobalUniforms.Projection);
            _material.ShaderProgram.Model.Set(model);
            _material.ShaderProgram.LightSpaceMatrix.Set(GlobalUniforms.LightSpaceMatrix);
            PbrShaderProgram pbrShader = _material.ShaderProgram as PbrShaderProgram;
            if(pbrShader!=null){
            pbrShader.FragColor.Set(new SimplexColor(255, 0, 0, 1));
            pbrShader.LightColor.Set(GlobalUniforms.LightColor);
            pbrShader.LightDir.Set(GlobalUniforms.LightDir);
           
            }
            _vao.Bind();
            _vao.BindAttribute(_material.ShaderProgram.InPosition, _vbo);
            if (_indexBuffer == null)
                _vao.DrawArrays(_drawMode, 0, _vbo.ElementCount);
            else
            {
                _vao.BindElementBuffer(_indexBuffer);
                _vao.DrawElements(_drawMode, _indexBuffer.ElementCount);
            }

        }

        public void RenderShadowMapSimple(in Matrix4 model, AlphaMode mode) 
        {

            ShadowMapSimpleProgramm.Instance.Use();
            if (_material.DoubleSided)
                GL.Disable(EnableCap.CullFace);
            else
                GL.Enable(EnableCap.CullFace);
            ShadowMapSimpleProgramm.Instance.Model.Set(model);
            ShadowMapSimpleProgramm.Instance.LightSpaceMatrix.Set(GlobalUniforms.LightSpaceMatrix);
            ShadowMapSimpleProgramm.Instance.View.Set(GlobalUniforms.View);
            ShadowMapSimpleProgramm.Instance.Projection.Set(GlobalUniforms.Projection);
            GL.BindTexture(TextureTarget.Texture2D, GlobalUniforms.ShadowMap);


            _vao.Bind();
            _vao.BindAttribute(ShadowMapSimpleProgramm.Instance.InPosition, _vbo);
            if (_indexBuffer == null)
                _vao.DrawArrays(_drawMode, 0, _vbo.ElementCount);
            else
            {
                _vao.BindElementBuffer(_indexBuffer);
                _vao.DrawElements(_drawMode, _indexBuffer.ElementCount);
            }
        }

        public void Dispose()
        {
            if (disposed)
                return;
            if (_vao != null)
                _vao.Dispose();
            if (_vbo != null)
                _vbo.Dispose();
            if (_indexBuffer != null)
                _indexBuffer.Dispose();
            if (_material != null)
                _material.Dispose();
            if (_tex != null)
                _tex.Dispose();
            if (_normals != null)
                _normals.Dispose();
            if (_tangents != null)
                _tangents.Dispose();
            disposed = true;
        }
    }

   

    /// <summary>
    /// base class for mesh information
    /// </summary>
    public class MeshComponent : ComponentBase,IDisposable
    {
        #region Private Fields

        private BoundingBox _bounds = new BoundingBox();

        private List<MeshPrimitive> _primitives = new List<MeshPrimitive>();
        private bool _drawBounds = true;

        private bool _disposed = false;

        #endregion Private Fields

        #region Public Properties
        public List<MeshPrimitive> Primitives { get => _primitives; set => _primitives = value; }
        public BoundingBox Bounds { get => _bounds; set => _bounds = value; }
        public bool DrawBounds { get => _drawBounds; set => _drawBounds = value; }

      
        ~MeshComponent()
        {
            if (!_disposed)
                Dispose();
        }

        #endregion Public Properties

        #region Public Methods

        public void Render(in Matrix4 model, AlphaMode mode)
        {
           
            foreach (MeshPrimitive primitive in Primitives)
            {
                if(primitive.Material.Blending==mode)
                   primitive.Render(model,mode);
            }
            if (_drawBounds)
                _bounds.Draw(model);
        }

        public void RenderShadowMapSimple(in Matrix4 model, AlphaMode mode)
        {

            foreach (MeshPrimitive primitive in Primitives)
            {
                if (primitive.Material.Blending == mode)
                    primitive.RenderShadowMapSimple(model, mode);
            }
          
        }

        public void Init()
        {
            foreach (MeshPrimitive primitive in _primitives)
            {
                primitive.Init();
            }
            CalculateBounds();
            _bounds.InitRenderer();
        }

        public void CalculateBounds()
        {
            Vector3 minBounds = new Vector3();
            Vector3 maxBounds = new Vector3();
            bool first = true;
            foreach (MeshPrimitive primitive in _primitives)
            {
                Vector3 primMin = primitive.Bounds.Min;
                Vector3 primMax = primitive.Bounds.Max;
                if (first)
                {
                    minBounds = primMin;
                    maxBounds = primMax;
                    first = false;
                }
                if (primMin.X < minBounds.X)
                    minBounds.X = primMin.X;
                if (primMax.X > maxBounds.X)
                    maxBounds.X = primMax.X;

                if (primMin.Y < minBounds.Y)
                    minBounds.Y = primMin.Y;
                if (primMax.Y > maxBounds.Y)
                    maxBounds.Y = primMax.Y;

                if (primMin.Z < minBounds.Z)
                    minBounds.Z = primMin.Z;
                if (primMax.Z > maxBounds.Z)
                    maxBounds.Z = primMax.Z;
            }
            Bounds.Min = minBounds;
            Bounds.Max = maxBounds;
        }

        /*public void DrawBounds()
        {
           // if (!_drawBounds)
           //     return;
           // _bounds.Draw(mvp);
        }*/

        public void Dispose()
        {
            if (_disposed)
                return;
            foreach (MeshPrimitive primitive in _primitives)
            {
                primitive.Dispose();
            }
        }

        #endregion Public Methods
    }
}