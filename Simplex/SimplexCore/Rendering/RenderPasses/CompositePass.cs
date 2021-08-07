using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using Simplex.Core.Scene;
using System;
using System.Collections.Generic;
using System.Text;
using ObjectTK.Buffers;
using ObjectTK.Textures;

namespace Simplex.Core.Rendering.RenderPasses
{
    class CompositePass : ColorDepthPass
    {
        VertexArray _quadVao;
        TexturedQuad _screenQuad;
        CompositionProgram _compositionProgram;
       
        private int _depthTextureLocation;

        public override int SortOrder => 40;

        protected override void OnInit(int width, int height)
        {
            if (_compositionProgram == null)
                _compositionProgram = SXProgramFactory.Create<CompositionProgram>();
            _compositionProgram.Use();
            base.OnInit(width, height);
            ClearMask = ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit;

            ScenePass scenePass = RenderPasses[typeof(ScenePass)] as ScenePass;
            Texture colorBuffer = scenePass.ColorBuffer;
            Texture normalBuffer = scenePass.NormalBuffer;
            Texture positionBuffer = scenePass.PositionBuffer;
            Texture emissiveBuffer = scenePass.EmissiveBuffer;
           
           
            _compositionProgram.Diffuse.BindTexture(TextureUnit.Texture0, colorBuffer);
            _compositionProgram.Normal.BindTexture(TextureUnit.Texture1, normalBuffer);
            _compositionProgram.Position.BindTexture(TextureUnit.Texture2, positionBuffer);
            _compositionProgram.Emissive.BindTexture(TextureUnit.Texture3, emissiveBuffer);
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
        }

        protected override void OnRender(Scene3D scene)
        {
            
            Camera cam = scene.CurrentCamera;
            Matrix4 projection = cam.getProjectionMatrix();
            Matrix4 view = cam.getViewMatrix();

            DirectionalShadowPass directionalShadowPass = RenderPasses[typeof(DirectionalShadowPass)] as DirectionalShadowPass;
            ScenePass scenePass = RenderPasses[typeof(ScenePass)] as ScenePass;
            Texture colorBuffer = scenePass.ColorBuffer;
            Texture normalBuffer = scenePass.NormalBuffer;
            Texture positionBuffer = scenePass.PositionBuffer;
            Texture emissiveBuffer = scenePass.EmissiveBuffer;
            int depthTexture = scenePass.DepthBuffer;


            colorBuffer.Bind(TextureUnit.Texture0);
            normalBuffer.Bind(TextureUnit.Texture1);
            positionBuffer.Bind(TextureUnit.Texture2);
            GL.ActiveTexture(TextureUnit.Texture3);
            GL.BindTexture(TextureTarget.Texture2D, directionalShadowPass.DepthBuffer);
            GL.ActiveTexture(TextureUnit.Texture4);
            GL.BindTexture(TextureTarget.Texture2D, depthTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            _compositionProgram.Use();
            _compositionProgram.ModelViewProjectionMatrix.Set(Matrix4.Identity);// * view * projection);
            GL.Uniform1(_depthTextureLocation, 4);
            _compositionProgram.Debug.Set(Mode == RenderMode.DEBUG ? true : false);
            _compositionProgram.ViewPortSize.Set(new Vector2(Width, Height));
            _compositionProgram.Ambient.Set(GlobalUniforms.AmbientColor);
            _compositionProgram.EyeDir.Set(cam.Forward);
            _quadVao.Bind();
            _quadVao.DrawArrays(PrimitiveType.TriangleStrip, 0, _screenQuad.VertexBuffer.ElementCount);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        protected override void OnResize(int width, int height)
        {
            OnInit(width,height);
        }
    }
}
