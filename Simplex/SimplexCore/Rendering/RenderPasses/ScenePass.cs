using Simplex.Core.Scene;
using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using ObjectTK.Buffers;
using ObjectTK.Textures;

namespace Simplex.Core.Rendering.RenderPasses
{
    class ScenePass : ColorDepthPass
    {
        private Texture _normalBuffer;
        private Texture _positionBuffer;
        private Texture _emissiveBuffer;
       

        public override int SortOrder => 20;

      
        public Texture NormalBuffer { get => _normalBuffer; set => _normalBuffer = value; }
        public Texture PositionBuffer { get => _positionBuffer; set => _positionBuffer = value; }
        public Texture EmissiveBuffer { get => _emissiveBuffer; set => _emissiveBuffer = value; }
       

        protected override void OnInit(int width, int height)
        {
            base.OnInit(width, height);
            ClearMask = ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit;
            InitTextures(width, height);
            EnableDepthTest = true;
        }

        private void InitTextures(int width,int height)
        {
            _normalBuffer = new Texture2D(SizedInternalFormat.Rgba32f, width, height);
            _positionBuffer = new Texture2D(SizedInternalFormat.Rgba32f, width, height);
            _emissiveBuffer = new Texture2D(SizedInternalFormat.Rgba32f, width, height);

            RenderBuffer.Attach(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment1, _normalBuffer);
            RenderBuffer.Attach(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment2, _positionBuffer);
            RenderBuffer.Attach(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment3, _emissiveBuffer);

            DrawBuffersEnum[] attachments = new DrawBuffersEnum[] {
                 DrawBuffersEnum.ColorAttachment0,
                  DrawBuffersEnum.ColorAttachment1,
                   DrawBuffersEnum.ColorAttachment2,
                   DrawBuffersEnum.ColorAttachment3

                   };
            GL.DrawBuffers(attachments.Length, attachments);
        }


        protected override void OnRender(Scene3D scene)
        {
            base.OnRender(scene);
            List<MeshNode> meshes = scene.RootNode.FindChildByType<MeshNode>();
            Camera cam = scene.CurrentCamera;
            GlobalUniforms.View = cam.getViewMatrix();
            GlobalUniforms.Projection = cam.getProjectionMatrix();
            
            
            _normalBuffer.Bind();
           
            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Blend);

            //sort meshes by distance to cam
            meshes.Sort(delegate (MeshNode left, MeshNode right)
            {
                Vector3 camPos = cam.Translation;
                Vector3 ldist = left.Translation - camPos;
                Vector3 rdist = right.Translation - camPos;
                return ldist.LengthFast > rdist.LengthFast ? 1 : -1;
            });
            //vp.M44=1;
            foreach (MeshNode mesh in meshes)
            {
                mesh.Render(AlphaMode.OPAQUE);
            }
            GL.Enable(EnableCap.Blend);

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            foreach (MeshNode mesh in meshes)
            {
                mesh.Render(AlphaMode.MASKED);
            }
            GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
            foreach (MeshNode mesh in meshes)
            {
                mesh.Render(AlphaMode.BLEND);
            }
        }

        protected override void OnResize(int width, int height)
        {
            base.OnResize(width,height);
            _normalBuffer.Dispose();
            _emissiveBuffer.Dispose();
            _positionBuffer.Dispose();
            InitTextures(width, height);
        }
    }
}
