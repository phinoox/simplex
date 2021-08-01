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


    public class BoundingBox : IDisposable
    {
        private static WireFrameProgram _shader;
        private bool disposed = false;
        private VertexArray _cubeVao;
        private WireFrameCube _cube;
        private SceneNode _parent;
        private static int _count =0;
        private int id = 0;
        private Vector3 _min = new Vector3(-0.5f);
        private Vector3 _max = new Vector3(0.5f);
        private Vector3 _offset = new Vector3();
        private Vector3 _scale = new Vector3(1);
        
        public Vector3 Max { get => _max; set => _max = value; }
        public Vector3 Min { get => _min; set => _min = value; }

        public Vector3 Center { get { return _min + ((_max - _min) * 0.5f); } }


        ~BoundingBox()
        {
            if (!disposed)
                Dispose();
        }

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

        public BoundingBox Scaled(Vector3 scale)
        {
            BoundingBox box = new BoundingBox();
            box.Max = _max * scale;
            box.Min = _min * scale;
            return box;
        }

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