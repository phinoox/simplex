using ObjectTK.Buffers;
using ObjectTK.Textures;
using ObjectTK.Tools.Shapes;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Simplex.Core.Scene;
using System.Collections.Generic;

namespace Simplex.Core.Rendering
{
    /// <summary>
    /// main class for rendering a 3D scene...I guess
    /// </summary>
    public class SXRenderer
    {

        Framebuffer _frameBuffer;
        Framebuffer _lightFrameBuffer;
        Framebuffer _shadowFrameBuffer;

        int _width, _height;
        VertexArray _quadVao;
        TexturedQuad _screenQuad;
        CompositionProgram _compositionProgram;
        Texture2D _colorBuffer;
        Texture2D _normalBuffer;
        Texture2D _positionBuffer;
        Texture2D _emissiveBuffer;

        Texture2D _lightBuffer;
        Texture2D _shadowBuffer;
        Renderbuffer _depthBuffer;


        #region Public Methods

        public void Init(int width, int height)
        {

            _width = width;
            _height = height;

            _frameBuffer = new Framebuffer();
            _frameBuffer.Bind(FramebufferTarget.Framebuffer);
            _depthBuffer = new Renderbuffer();
            _depthBuffer.Init(RenderbufferStorage.DepthComponent, width, height);
            _frameBuffer.Attach(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, _depthBuffer);

            _colorBuffer = new Texture2D(SizedInternalFormat.Rgba8, width, height);
            _normalBuffer = new Texture2D(SizedInternalFormat.Rgba8, width, height);
            _positionBuffer = new Texture2D(SizedInternalFormat.Rgba8, width, height);
            _emissiveBuffer = new Texture2D(SizedInternalFormat.Rgba8, width, height);



            _frameBuffer.Attach(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, _colorBuffer);
            _frameBuffer.Attach(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment1, _normalBuffer);
            _frameBuffer.Attach(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment2, _positionBuffer);
            _frameBuffer.Attach(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment3, _emissiveBuffer);

            DrawBuffersEnum[] attachments = new DrawBuffersEnum[] {
                 DrawBuffersEnum.ColorAttachment0,
                  DrawBuffersEnum.ColorAttachment1,
                   DrawBuffersEnum.ColorAttachment2,
                   DrawBuffersEnum.ColorAttachment3
                   };
            GL.DrawBuffers(attachments.Length, attachments);

            Framebuffer.Unbind(FramebufferTarget.Framebuffer);
            if (_compositionProgram == null)
                _compositionProgram = SXProgramFactory.Create<CompositionProgram>();
            _compositionProgram.Use();
            _compositionProgram.Diffuse.BindTexture(TextureUnit.Texture0, _colorBuffer);
            _compositionProgram.Normal.BindTexture(TextureUnit.Texture1, _normalBuffer);
            _compositionProgram.Position.BindTexture(TextureUnit.Texture2, _positionBuffer);
            _compositionProgram.Position.BindTexture(TextureUnit.Texture3, _emissiveBuffer);

            if (_screenQuad == null)
            {
                _screenQuad = new TexturedQuad();
                _screenQuad.UpdateBuffers();
                _quadVao = new VertexArray();
                _quadVao.Bind();
                _quadVao.BindAttribute(_compositionProgram.InPosition, _screenQuad.VertexBuffer);
                _quadVao.BindAttribute(_compositionProgram.InTexCoord, _screenQuad.TexCoordBuffer);
            }


            //initializing lightFrameBuffer
            _lightFrameBuffer = new Framebuffer();
            _lightFrameBuffer.Bind(FramebufferTarget.Framebuffer);
            _lightBuffer = new Texture2D(SizedInternalFormat.Rgba8, width, height);
            _lightFrameBuffer.Attach(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, _lightBuffer);
            Framebuffer.Unbind(FramebufferTarget.Framebuffer);

            //initializing lightFrameBuffer
            _shadowFrameBuffer = new Framebuffer();
            _shadowFrameBuffer.Bind(FramebufferTarget.Framebuffer);
            _shadowBuffer = new Texture2D(SizedInternalFormat.Rgba8, width, height);
            _shadowFrameBuffer.Attach(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, _shadowBuffer);
            Framebuffer.Unbind(FramebufferTarget.Framebuffer);
        }

        private void RenderScene(Scene3D scene)
        {
            List<MeshNode> meshes = scene.RootNode.FindChildByType<MeshNode>();
            Camera cam = scene.CurrentCamera;
            GlobalUniforms.View = cam.getViewMatrix();
            GlobalUniforms.Projection = cam.getProjectionMatrix();
            _frameBuffer.Bind(FramebufferTarget.Framebuffer);
            _colorBuffer.Bind();
            _normalBuffer.Bind();
            GL.Viewport(0, 0, _width, _height);
            GL.ClearColor(Color.LightSkyBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            //vp.M44=1;
            foreach (MeshNode mesh in meshes)
            {
                mesh.Render(cam);
            }
            Framebuffer.Unbind(FramebufferTarget.Framebuffer);
            //_colorBuffer.GenerateMipMaps();
            //_normalBuffer.GenerateMipMaps();
            //_positionBuffer.GenerateMipMaps();
        }

        private void RenderLightPass(Scene3D scene)
        {
            Camera cam = scene.CurrentCamera;
            Matrix4 view = cam.getViewMatrix();
            Matrix4 projection = cam.getProjectionMatrix();
            _lightFrameBuffer.Bind(FramebufferTarget.Framebuffer);
            List<LightNode> lights = scene.RootNode.FindChildByType<LightNode>();
            foreach (LightNode light in lights)
            {
                if (light.LightType == LightTypes.DIRECTIONAL)
                {
                    //GlobalUniforms.LightDir = light.Forward;//(Matrix4.Identity *view * projection * new Vector4(light.Forward, 0)).Xyz;
                }
            }
            Framebuffer.Unbind(FramebufferTarget.Framebuffer);
        }

        private void RenderShadowPass(Scene3D scene)
        {
            _shadowFrameBuffer.Bind(FramebufferTarget.Framebuffer);
            List<LightNode> lights = scene.RootNode.FindChildByType<LightNode>();
            foreach (LightNode light in lights)
            {
                if (!light.CastsShadow)
                    continue;

            }
            Framebuffer.Unbind(FramebufferTarget.Framebuffer);
        }

        private void RenderCompositeQuad(Scene3D scene)
        {

            Camera cam = scene.CurrentCamera;
            Matrix4 projection = cam.getProjectionMatrix();
            Matrix4 view = cam.getViewMatrix();
            GL.Viewport(0, 0, _width, _height);
            GL.ClearColor(Color.MidnightBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //GL.Disable(EnableCap.DepthTest);
            _colorBuffer.Bind(TextureUnit.Texture0);
            _normalBuffer.Bind(TextureUnit.Texture1);
            _positionBuffer.Bind(TextureUnit.Texture2);
            _emissiveBuffer.Bind(TextureUnit.Texture3);
            _compositionProgram.Use();
            _compositionProgram.ModelViewProjectionMatrix.Set(Matrix4.Identity);// * view * projection);

            _quadVao.Bind();
            _quadVao.DrawArrays(PrimitiveType.TriangleStrip, 0, _screenQuad.VertexBuffer.ElementCount);
        }

        /// <summary>
        /// renders the scene
        /// </summary>
        /// <param name="scene">the scene to be rendered</param>
        public void Render(Scene3D scene)
        {
            GL.ClearColor(Color.MidnightBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            RenderScene(scene);
            RenderLightPass(scene);
            RenderShadowPass(scene);
            RenderCompositeQuad(scene);
        }

        public void ResizeFrameBuffer(int width, int height)
        {
            _frameBuffer.Dispose();
            _lightFrameBuffer.Dispose();
            _shadowFrameBuffer.Dispose();
            _colorBuffer.Dispose();
            _normalBuffer.Dispose();
            _depthBuffer.Dispose();
            _emissiveBuffer.Dispose();
            _positionBuffer.Dispose();
            _lightBuffer.Dispose();
            _shadowBuffer.Dispose();
            Init(width, height);
        }

        #endregion Public Methods
    }
}