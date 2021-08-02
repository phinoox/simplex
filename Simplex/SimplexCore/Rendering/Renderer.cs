using ObjectTK.Buffers;
using ObjectTK.Textures;
using Simplex.Core.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Simplex.Core.Scene;
using System.Collections.Generic;
using Simplex.Core.Components;
using Simplex.Core.Rendering.Programs;

namespace Simplex.Core.Rendering
{
    /// <summary>
    /// Mode in which the scene object should be rendered
    /// </summary>
    public enum RenderMode
    {
        /// <summary>
        /// fully shaded
        /// </summary>
        SHADED,
        /// <summary>
        /// no lighting
        /// </summary>
        UNLIT,
        /// <summary>
        /// additional debug info
        /// </summary>
        DEBUG,
        /// <summary>
        /// only wireframe
        /// </summary>
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
        private bool _needShaderReload = false;
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

        int _directionalDepthTexture;
        int _depthTexture;
        int _depthTextureLocation;

        /// <summary>
        /// Mode in which the scene object should be rendered
        /// </summary>
        public RenderMode Mode { get => mode; set => mode = value; }
        public bool NeedShaderReload { get => _needShaderReload; set => _needShaderReload = value; }


        #region Public Methods

        /// <summary>
        /// initializes the renderer
        /// should be called after opengl context has been created
        /// </summary>
        /// <param name="width">width of the visual area</param>
        /// <param name="height">height of the visual area</param>
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
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Depth24Stencil8, width, height, 0, OpenTK.Graphics.OpenGL4.PixelFormat.DepthComponent, PixelType.Float, System.IntPtr.Zero);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, _depthTexture, 0);

            GL.BindTexture(TextureTarget.Texture2D, 0);


            _colorBuffer = new Texture2D(SizedInternalFormat.Rgba8, width, height);
            _normalBuffer = new Texture2D(SizedInternalFormat.Rgba32f, width, height);
            _positionBuffer = new Texture2D(SizedInternalFormat.Rgba32f, width, height);
            _emissiveBuffer = new Texture2D(SizedInternalFormat.Rgba32f, width, height);



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
            _depthTextureLocation = GL.GetUniformLocation(_compositionProgram.Handle, "Depth");

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

        /// <summary>
        /// renders the scene
        /// </summary>
        /// <param name="scene">the scene to be rendered</param>
        public void Render(Scene3D scene)
        {
            if (_needShaderReload)
            {
                SXProgramFactory.ReloadShaders();
                _needShaderReload = false;
                ResizeFrameBuffer(_width, _height);
                return;
            }
            GL.ClearColor(Color4.MidnightBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            RenderSky(scene);
            RenderDirectionalLight(scene);
            RenderScene(scene);

            // RenderLightPass(scene);
            RenderShadowPass(scene);
            RenderCompositeQuad(scene);
        }

        private void InitPorgrams()
        {
            Framebuffer.Unbind(FramebufferTarget.Framebuffer);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            _compositionProgram.Use();
            _compositionProgram.Diffuse.BindTexture(TextureUnit.Texture0, _colorBuffer);
            _compositionProgram.Normal.BindTexture(TextureUnit.Texture1, _normalBuffer);
            _compositionProgram.Position.BindTexture(TextureUnit.Texture2, _positionBuffer);
            _compositionProgram.Emissive.BindTexture(TextureUnit.Texture3, _emissiveBuffer);
            _depthTextureLocation = GL.GetUniformLocation(_compositionProgram.Handle, "Depth");
            _quadVao.Bind();
            _quadVao.BindAttribute(_compositionProgram.InPosition, _screenQuad.VertexBuffer);
            _quadVao.BindAttribute(_compositionProgram.InTexCoord, _screenQuad.TexCoordBuffer);
        }

        private void RenderSky(Scene3D scene)
        {
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
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
            GL.ClearColor(Color4.LightSkyBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Blend);
            //GL.BlendFuncSeparate(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha,BlendingFactorSrc.One,BlendingFactorDest.OneMinusSrcAlpha);  

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
            //GL.BlendFuncSeparate(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha,BlendingFactorSrc.One,BlendingFactorDest.OneMinusSrcAlpha);  

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

            Framebuffer.Unbind(FramebufferTarget.Framebuffer);
            //_colorBuffer.GenerateMipMaps();
            //_normalBuffer.GenerateMipMaps();
            //_positionBuffer.GenerateMipMaps();
        }

        private void RenderDirectionalLight(Scene3D scene)
        {
            LightNode lightNode = scene.DirectionalLight;
            List<MeshNode> meshes = scene.RootNode.FindChildByType<MeshNode>();
            Camera cam = scene.CurrentCamera;
            DirectionalLight light = lightNode.Light as DirectionalLight;

            for (int cascade = 0; cascade < light.CascadeDepths.Length; ++cascade)
            {
                float near = cascade == 0 ? 0 : light.CascadeDepths[cascade - 1] + 0.001f;
                float far = light.CascadeDepths[cascade];

                _directionalDepthTexture = light.DepthTextures[cascade];
                float halfClip = cam.FarClip * 0.5f;
                Vector3 clipCenter = cam.Translation + (new Vector3(cam.Forward.X, 0, cam.Forward.Z) * halfClip);
                Vector3 pos = cam.Translation - (lightNode.Forward * light.CascadeDepths[cascade]);
                Matrix4 view = Matrix4.LookAt(pos, pos + lightNode.Forward, WorldDefaults.Up);
                Matrix4 proj = light.GetProjectionMatrix(cascade);
                GlobalUniforms.LightSpaceMatrix = view * proj;
                GlobalUniforms.LightDir = lightNode.Forward;
                GL.Enable(EnableCap.DepthTest);
                light.BindBuffer(cascade);
                foreach (MeshNode mesh in meshes)
                {
                    mesh.RenderShadowMapSimple(AlphaMode.OPAQUE);
                }
                GL.Enable(EnableCap.Blend);
                //GL.BlendFuncSeparate(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha,BlendingFactorSrc.One,BlendingFactorDest.OneMinusSrcAlpha);  

                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                foreach (MeshNode mesh in meshes)
                {
                    mesh.RenderShadowMapSimple(AlphaMode.MASKED);
                }
                GL.BlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
                foreach (MeshNode mesh in meshes)
                {
                    mesh.RenderShadowMapSimple(AlphaMode.BLEND);
                }



                light.UnbindBuffer();
            }
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
                    List<MeshNode> meshes = scene.RootNode.FindChildByType<MeshNode>();
                    foreach (MeshNode mesh in meshes)
                    {
                        mesh.Render(AlphaMode.OPAQUE);
                    }
                }
                else
                {
                    light.Render(cam);
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
            GL.ClearColor(Color4.MidnightBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //GL.Disable(EnableCap.Blend);
            _colorBuffer.Bind(TextureUnit.Texture0);
            _normalBuffer.Bind(TextureUnit.Texture1);
            _positionBuffer.Bind(TextureUnit.Texture2);
            //_emissiveBuffer.Bind(TextureUnit.Texture3);
            GL.ActiveTexture(TextureUnit.Texture3);
            GL.BindTexture(TextureTarget.Texture2D, _directionalDepthTexture);
            GL.ActiveTexture(TextureUnit.Texture4);
            GL.BindTexture(TextureTarget.Texture2D, _depthTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, (int)TextureCompareMode.None);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.DepthTextureMode, (int)PixelFormat.Luminance);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareFunc, (int)All.Lequal);
            _compositionProgram.Use();
            _compositionProgram.ModelViewProjectionMatrix.Set(Matrix4.Identity);// * view * projection);
            GL.Uniform1(_depthTextureLocation, 4);
            //_compositionProgram.Depth.Set(dt_loc);
            _compositionProgram.Debug.Set(mode == RenderMode.DEBUG ? true : false);
            _compositionProgram.ViewPortSize.Set(new Vector2(_width, _height));
            _compositionProgram.Ambient.Set(GlobalUniforms.AmbientColor);
            _quadVao.Bind();
            _quadVao.DrawArrays(PrimitiveType.TriangleStrip, 0, _screenQuad.VertexBuffer.ElementCount);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

       

        /// <summary>
        /// needs to be called whenever the viewport has been resized
        /// </summary>
        /// <param name="width">new widht of the viewport</param>
        /// <param name="height">new height of the viewport</param>
        public void ResizeFrameBuffer(int width, int height)
        {
            if (width == 0 || height == 0)
                return;
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