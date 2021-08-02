using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using Simplex.Core.Scene;
using System;
using System.Collections.Generic;
using System.Text;
using ObjectTK.Buffers;
using Simplex.Core.Rendering;
using Simplex.Core.Rendering.Programs;

namespace Simplex.Core.Components
{
    public class DirectionalLight : LightBase
    {
        int[] _depthTextures;
        int _shadowmapSize = 2048;
        int[] cascadeDepths = new int[] { 10, 30, 100 };
        Framebuffer _frameBuffer;

        public int[] DepthTextures { get => _depthTextures; set => _depthTextures = value; }
        public int[] CascadeDepths { get => cascadeDepths; set => cascadeDepths = value; }

        
        protected override void OnRender(Matrix4 mvp)
        {
            base.OnRender(mvp);

        }

        protected override void OnCreate()
        {
            base.OnCreate();
            InitDepthBuffer();
        }

        private void InitDepthBuffer()
        {
            _frameBuffer = new Framebuffer();
            _frameBuffer.Bind(FramebufferTarget.Framebuffer);

            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);
            Framebuffer.Unbind(FramebufferTarget.Framebuffer);
            _depthTextures = new int[cascadeDepths.Length];
            for (int i = 0; i < cascadeDepths.Length; ++i)
            {
                int depthTexture = GL.GenTexture();
                _depthTextures[i] = depthTexture;

                ShadowMapSimpleProgramm.Instance.Use();
                ShadowMapSimpleProgramm.Instance.ShadowMap.Set(depthTexture);
                GL.BindTexture(TextureTarget.Texture2D, depthTexture);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Depth24Stencil8, _shadowmapSize, _shadowmapSize, 0, OpenTK.Graphics.OpenGL4.PixelFormat.DepthComponent, PixelType.Float, System.IntPtr.Zero);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                GL.BindTexture(TextureTarget.Texture2D, 0);
            }
            GlobalUniforms.ShadowMap = _depthTextures[0];
        }

        public Matrix4 GetProjectionMatrix(int cascade)
        {
            float size = cascadeDepths[cascade];
            float near = cascade == 0 ? 0 : cascadeDepths[cascade - 1] + 0.001f;
            float far = cascadeDepths[cascade];
            return Matrix4.CreateOrthographic(size, size, near, far);
        }

        public void BindBuffer(int cascade)
        {
            GL.Viewport(0, 0, _shadowmapSize, _shadowmapSize);
            _frameBuffer.Bind(FramebufferTarget.Framebuffer);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, _depthTextures[cascade], 0);
            GL.Clear(ClearBufferMask.DepthBufferBit);
        }

        public void UnbindBuffer()
        {
            Framebuffer.Unbind(FramebufferTarget.Framebuffer);
        }
    }
}
