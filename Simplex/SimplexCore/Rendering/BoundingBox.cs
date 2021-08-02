using Simplex.Core.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Simplex.Core.Rendering.Programs;
using ObjectTK.Buffers;
using Simplex.Core.Scene;
using System;

namespace Simplex.Core.Rendering
{

    /// <summary>
    /// a standard BB implementation
    /// if ti should be able to get rendered in wireframe, don't forget to call the init render function
    /// </summary>
    public class BoundingBox : IDisposable
    {
        private static WireFrameProgram _shader;
        private bool disposed = false;
        private VertexArray _cubeVao;
        private WireFrameCube _cube;
        private static int _count =0;
        private int id = 0;
        private Vector3 _min = new Vector3(-0.5f);
        private Vector3 _max = new Vector3(0.5f);
        
        public Vector3 Max { get => _max; set => _max = value; }
        public Vector3 Min { get => _min; set => _min = value; }

        public Vector3 Center { get { return _min + ((_max - _min) * 0.5f); } }

        /// <summary>
        /// calls dispose if that did't already happen
        /// </summary>
        ~BoundingBox()
        {
            if (!disposed)
                Dispose();
        }

        /// <summary>
        /// initializes the wireframe visualzitation
        /// </summary>
        public void InitRenderer()
        {
           // _parent = parent;
            id = ++_count;

            if (_shader == null)
            {
                _shader = SXProgramFactory.Create<WireFrameProgram>();
               
            }

            _cube = new WireFrameCube();
            _cube.UpdateBuffers();
            _cube.DefaultMode = OpenTK.Graphics.OpenGL4.PrimitiveType.LineLoop;
            _cubeVao = new VertexArray();
            _cubeVao.Bind();
            _cubeVao.BindAttribute(_shader.InPosition, _cube.VertexBuffer);
            _cubeVao.BindElementBuffer(_cube.IndexBuffer);

        }

        /// <summary>
        /// returns a new boundig box scaled by the given factor
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        public BoundingBox Scaled(Vector3 scale)
        {
            BoundingBox box = new BoundingBox();
            box.Max = _max * scale;
            box.Min = _min * scale;
            return box;
        }

        /// <summary>
        /// draws the wireframe visualization
        /// </summary>
        /// <param name="model"></param>
        public void Draw(in Matrix4 model)
        {
            Vector3 size = (Max - Min)*0.5f;
            Matrix4 scale = Matrix4.CreateScale(size);
            Matrix4 translation = Matrix4.CreateTranslation(Center);
            Matrix4 finalModel = scale * translation * model;
           


            GL.Disable(EnableCap.CullFace);
            GL.LineWidth(2);
            _shader.Use();
            _shader.View.Set(GlobalUniforms.View);
            _shader.Projection.Set(GlobalUniforms.Projection);
            _shader.WireColor.Set(Color4.Orange);
            _shader.Model.Set(finalModel);
            _cubeVao.Bind();
            _cubeVao.DrawElements(_cube.DefaultMode, _cube.IndexBuffer.ElementCount);
            GL.Enable(EnableCap.CullFace);
        }

        public void Dispose()
        {
            if (disposed)
                return;
            if (_cube != null)
                _cube.Dispose();
            if (_cubeVao != null)
                _cubeVao.Dispose();
            
            disposed = true;
        }
    }

}