using Simplex.Core.Scene;
using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using Simplex.Core.Components;
using OpenTK.Mathematics;
using System.Linq;
using ObjectTK.Textures;
using Simplex.Core.Rendering.Programs;

namespace Simplex.Core.Rendering.RenderPasses
{
    class DirectionalShadowPass : DepthOnlyPass
    {
       // private int _directionalDepthTexture;
        int[] _depthTextures;
        int[] _cascadeDepths = new int[] { 10, 30, 100 };

        public override int SortOrder => 0;

        //public int DepthTexture { get => _directionalDepthTexture; set => _directionalDepthTexture = value; }

        protected override void OnInit(int width, int height)
        {
            ResizePolicy = ResizePolicies.Fixed;
            
            base.OnInit(width, height);
            GlobalUniforms.ShadowMap = DepthBuffer;
            ClearMask = ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit | ClearBufferMask.StencilBufferBit;
            EnableDepthTest = true;
           // InitDepthTextures();
        }

        private void InitDepthTextures()
        {
            
            
            {
                DepthBuffer = GL.GenTexture();
                

                ShadowMapSimpleProgramm.Instance.Use();
                ShadowMapSimpleProgramm.Instance.ShadowMap.Set(DepthBuffer);
                
                GL.BindTexture(TextureTarget.Texture2D, DepthBuffer);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Depth24Stencil8, FramebufferSize, FramebufferSize, 0, OpenTK.Graphics.OpenGL4.PixelFormat.DepthComponent, PixelType.Float, System.IntPtr.Zero);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, DepthBuffer, 0);
                GL.BindTexture(TextureTarget.Texture2D, 0);
            }
            GlobalUniforms.ShadowMap = DepthBuffer;// = _depthTextures[0];
            return;

            _depthTextures = new int[_cascadeDepths.Length];
            for (int i = 0; i < 1; ++i)
            {
                int depthTexture = GL.GenTexture();
                _depthTextures[i] = depthTexture;

                ShadowMapSimpleProgramm.Instance.Use();
                ShadowMapSimpleProgramm.Instance.ShadowMap.Set(depthTexture);
                GL.BindTexture(TextureTarget.Texture2D, depthTexture);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Depth24Stencil8, FramebufferSize, FramebufferSize, 0, OpenTK.Graphics.OpenGL4.PixelFormat.DepthComponent, PixelType.Float, System.IntPtr.Zero);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, DepthBuffer, 0);
                GL.BindTexture(TextureTarget.Texture2D, 0);
            }
            GlobalUniforms.ShadowMap = DepthBuffer = _depthTextures[0];
        }


        private Matrix4 GetProjectionMatrix(int cascade)
        {
            float size = _cascadeDepths[cascade];
            float near = cascade == 0 ? 0 : _cascadeDepths[cascade - 1] + 0.001f;
            float far = _cascadeDepths[cascade];
            return Matrix4.CreateOrthographic(size, size, near, far);
        }

        protected override void OnRender(Scene3D scene)
        {
            base.OnRender(scene);
            LightNode lightNode = scene.DirectionalLight;
            Camera cam = scene.CurrentCamera;
            float halfClip = cam.FarClip * 0.5f;
            Vector3 clipCenter = cam.Translation + (new Vector3(cam.Forward.X, 0, cam.Forward.Z) * halfClip);
            Vector3 pos = cam.Translation;// - (lightNode.Forward * cam.FarClip);

            List<MeshNode> meshes = scene.RootNode.FindChildByType<MeshNode>().Where(x => (x.Translation - lightNode.Translation).Length < cam.FarClip).ToList();
            meshes.Sort(delegate (MeshNode left, MeshNode right)
            {
                Vector3 camPos = pos;
                Vector3 ldist = left.Translation - camPos;
                Vector3 rdist = right.Translation - camPos;
                return ldist.LengthFast > rdist.LengthFast ? 1 : -1;
            });

            DirectionalLight light = lightNode.Light as DirectionalLight;
            ShadowMapSimpleProgramm.Instance.Use();
            ShadowMapSimpleProgramm.Instance.ShadowMap.Set(DepthBuffer);

            {
                
                //Matrix4 view = Matrix4.LookAt(pos, pos + lightNode.Forward, WorldDefaults.Up);
                //Matrix4 proj = GetProjectionMatrix(0);

                Matrix4 view = Matrix4.LookAt(-lightNode.Forward * 10.0f, Vector3.Zero, WorldDefaults.Up);
                Matrix4 proj = Matrix4.CreateOrthographic(40.0f, 40.0f, -15.0f, 20.0f);

                GlobalUniforms.LightSpaceMatrix = view * proj;
                GlobalUniforms.LightDir = lightNode.Forward;
                GL.Enable(EnableCap.DepthTest);
                GL.BindTexture(TextureTarget.Texture2D, DepthBuffer);
                foreach (MeshNode mesh in meshes)
                {
                    mesh.RenderShadowMapSimple(AlphaMode.OPAQUE);
                }
                GL.Enable(EnableCap.Blend);

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

            }
            return;

            for (int cascade = 0; cascade < 1; ++cascade)
            {
                float near = cascade == 0 ? 0 : _cascadeDepths[cascade - 1] + 0.001f;
                float far = _cascadeDepths[cascade];

                DepthBuffer = _depthTextures[cascade];
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, DepthBuffer, 0);
                //GL.Clear(ClearBufferMask.DepthBufferBit);
                Matrix4 view = Matrix4.LookAt(pos, pos + lightNode.Forward, WorldDefaults.Up);
                Matrix4 proj = GetProjectionMatrix(cascade);
                GlobalUniforms.LightSpaceMatrix = view * proj;
                GlobalUniforms.LightDir = lightNode.Forward;
                GL.Enable(EnableCap.DepthTest);
                
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
                
            }
        }

        protected override void OnResize(int width, int height)
        {
            base.OnResize(width, height);
            GlobalUniforms.ShadowMap = DepthBuffer;
            // OnInit(width,height);
        }
    }
}
