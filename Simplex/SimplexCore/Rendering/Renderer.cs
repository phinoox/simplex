using ObjectTK.Buffers;
using ObjectTK.Textures;
using ObjectTK.Tools.Shapes;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Simplex.Core.Scene;
using System.Collections.Generic;

namespace Simplex.Core.Rendering
{
    public enum RenderMode
    {
        SHADED,
        UNLIT,
        DEBUG,
        WIREFRAME
    }



    /// <summary>
    /// main class for rendering a 3D scene...I guess
    /// </summary>
    public class SXRenderer
    {
        private RenderMode mode = RenderMode.SHADED;
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
        int _depthTexture;
        int _depthTextureLocation;

        public RenderMode Mode { get => mode; set => mode = value; }


        #region Public Methods

        public void Init(int width, int height)
        {

            _width = width;
            _height = height;

            _frameBuffer = new Framebuffer();
            _frameBuffer.Bind(FramebufferTarget.Framebuffer);
            //_depthBuffer = new Renderbuffer();
            //_depthBuffer.Init(RenderbufferStorage.DepthComponent, width, height);
            //_frameBuffer.Attach(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, _depthBuffer);
            
            //depthbuffer manual

            _depthTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _depthTexture);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareFunc, (int)All.Lequal);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, width, height, 0, OpenTK.Graphics.OpenGL.PixelFormat.DepthComponent, PixelType.Float, System.IntPtr.Zero);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, _depthTexture, 0);

            GL.BindTexture(TextureTarget.Texture2D, 0);
            
           
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
                   DrawBuffersEnum.ColorAttachment3,
                    DrawBuffersEnum.ColorAttachment4
                   };
            GL.DrawBuffers(attachments.Length, attachments);

            Framebuffer.Unbind(FramebufferTarget.Framebuffer);
            if (_compositionProgram == null)
                _compositionProgram = SXProgramFactory.Create<CompositionProgram>();
            _compositionProgram.Use();
            _compositionProgram.Diffuse.BindTexture(TextureUnit.Texture0, _colorBuffer);
            _compositionProgram.Normal.BindTexture(TextureUnit.Texture1, _normalBuffer);
            _compositionProgram.Position.BindTexture(TextureUnit.Texture2, _positionBuffer);
            _compositionProgram.Emissive.BindTexture(TextureUnit.Texture3, _emissiveBuffer);
             _depthTextureLocation = GL.GetUniformLocation(_compositionProgram.Handle,"Depth");
           
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
            //GL.Disable(EnableCap.Blend);
            _colorBuffer.Bind(TextureUnit.Texture0);
            _normalBuffer.Bind(TextureUnit.Texture1);
            _positionBuffer.Bind(TextureUnit.Texture2);
            _emissiveBuffer.Bind(TextureUnit.Texture3);
            GL.ActiveTexture(TextureUnit.Texture4);
            GL.BindTexture(TextureTarget.Texture2D,_depthTexture);
           GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
           GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
           //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
           //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
           //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareFunc, (int)All.Lequal);
            _compositionProgram.Use();
            _compositionProgram.ModelViewProjectionMatrix.Set(Matrix4.Identity);// * view * projection);
             GL.Uniform1(_depthTextureLocation,4);
            //_compositionProgram.Depth.Set(dt_loc);
            _compositionProgram.Debug.Set(mode == RenderMode.DEBUG ? true : false);
            _quadVao.Bind();
            _quadVao.DrawArrays(PrimitiveType.TriangleStrip, 0, _screenQuad.VertexBuffer.ElementCount);
             GL.ActiveTexture(TextureUnit.Texture0);
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
            //_depthBuffer.Dispose();
            GL.DeleteTexture(_depthTexture);
            _emissiveBuffer.Dispose();
            _positionBuffer.Dispose();
            _lightBuffer.Dispose();
            _shadowBuffer.Dispose();
            Init(width, height);
        }

        #endregion Public Methods
    }
}