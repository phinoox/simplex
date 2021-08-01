using ObjectTK.Shaders.Sources;
using ObjectTK.Shaders.Variables;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simplex.Core.Rendering.Programs
{
    [VertexShaderSource("ShadowMapSimple.Vertex")]
    [FragmentShaderSource("ShadowMapSimple.Fragment")]
    public class ShadowMapSimpleProgramm : ProgramBase
    {
        private static ShadowMapSimpleProgramm _instance;
        public TextureUniform ShadowMap { get; set; }

        public static ShadowMapSimpleProgramm Instance { 
            get
            {
                if (_instance == null)
                    CreateInstance();
                return _instance;
            }
        }

        private static void CreateInstance()
        {
            _instance = SXProgramFactory.Create<ShadowMapSimpleProgramm>();
        }

        

       
    }
}
