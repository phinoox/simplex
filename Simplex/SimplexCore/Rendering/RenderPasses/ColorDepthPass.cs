using ObjectTK.Textures;
using OpenTK.Graphics.OpenGL4;
using Simplex.Core.Scene;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simplex.Core.Rendering.RenderPasses
{
    public abstract class ColorDepthPass : DepthOnlyPass
    {
        private Texture _colorBuffer;

        public Texture ColorBuffer { get => _colorBuffer; set => _colorBuffer = value; }

        protected override void OnInit(int width, int height)
        {
            base.OnInit(width, height);
            InitColorBuffer(width, height);
        }

        private void InitColorBuffer(int width,int height)
        {
            if (ResizePolicy == ResizePolicies.Fixed)
                width = height = FramebufferSize;
                _colorBuffer = new Texture2D(SizedInternalFormat.Rgba32f, width, height);
            RenderBuffer.Attach(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, _colorBuffer);
            DrawBuffersEnum[] attachments = new DrawBuffersEnum[] {
                 DrawBuffersEnum.ColorAttachment0,
                   };
            GL.DrawBuffers(attachments.Length, attachments);
        }

        protected override void OnRender(Scene3D scene)
        {
            base.OnRender(scene);
            _colorBuffer.Bind();
        }

        protected override void OnResize(int width, int height)
        {
            base.OnResize(width, height);
            _colorBuffer.Dispose();
            InitColorBuffer(width, height);
        }


    }
}
