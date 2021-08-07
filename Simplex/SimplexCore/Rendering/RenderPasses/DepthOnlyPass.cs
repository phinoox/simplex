using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using Simplex.Core.Scene;

namespace Simplex.Core.Rendering.RenderPasses
{
    public abstract class DepthOnlyPass : RenderPass
    {
        private int _depthBuffer;
        private int _depthTextureLocation;
        public int DepthBuffer { get => _depthBuffer; set => _depthBuffer = value; }

        protected override void OnInit(int width, int height)
        {
            base.OnInit(width, height);
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);
            InitDepthTexture(width, height);
        }

        private void InitDepthTexture(int width,int height)
        {
            if (ResizePolicy == ResizePolicies.Fixed)
                width = height = FramebufferSize;
            _depthBuffer = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _depthBuffer);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Depth24Stencil8, width, height, 0, OpenTK.Graphics.OpenGL4.PixelFormat.DepthComponent, PixelType.Float, System.IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, _depthBuffer, 0);
          
        }


        protected override void OnResize(int width, int height)
        {
            GL.DeleteTexture(_depthBuffer);
            InitDepthTexture(width, height);
        }

    }
}
