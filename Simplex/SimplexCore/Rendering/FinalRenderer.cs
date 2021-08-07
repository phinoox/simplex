using ObjectTK.Buffers;
using ObjectTK.Textures;
using OpenTK.Graphics.OpenGL4;
using Simplex.Core.Rendering.Programs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simplex.Core.Rendering
{
    public class FinalRenderer
    {

        VertexArray _quadVao;
        TexturedQuad _screenQuad;
        FinalQuadProgram _finalProgram;
        Texture _diffuse;

        public void Init(Texture diffuse)
        {
            _diffuse = diffuse;
            if(_finalProgram == null)
            {
                _finalProgram = SXProgramFactory.Create<FinalQuadProgram>();
               
            }
            _finalProgram.Use();
            _finalProgram.Diffuse.BindTexture(OpenTK.Graphics.OpenGL4.TextureUnit.Texture0, _diffuse);

            if (_screenQuad == null)
            {
                _screenQuad = new TexturedQuad();
                _screenQuad.UpdateBuffers();
                _quadVao = new VertexArray();
                _quadVao.Bind();
                _quadVao.BindAttribute(_finalProgram.InPosition, _screenQuad.VertexBuffer);
                _quadVao.BindAttribute(_finalProgram.InTexCoord, _screenQuad.TexCoordBuffer);
            }
        }

        public void Render()
        {
            _finalProgram.Use();
            _diffuse.Bind();
            _quadVao.Bind();
            _quadVao.DrawArrays(PrimitiveType.TriangleStrip, 0, _screenQuad.VertexBuffer.ElementCount);
        }

    }
}
