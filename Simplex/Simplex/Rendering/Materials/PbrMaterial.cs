using System;
using OpenTK.Graphics.OpenGL;
using ObjectTK.Shaders;
using ObjectTK.Textures;

namespace Simplex.Rendering
{
    /// <summary>
    /// basic class for pbr material
    /// tries to be compatible with gltf materials
    /// </summary>
    public class PbrMaterial : MaterialBase
    {
        #region Private Fields
        private bool initialized = false;
        private ObjectTK.Textures.Texture _albedo;
        private float _basecolorFactor;
        private float _emissiveFactor;
        private ObjectTK.Textures.Texture _emissiveMap;
        private float _metalicness;
        private ObjectTK.Textures.Texture _metalTexture;
        private float _normalFactor;
        private ObjectTK.Textures.Texture _normalMap;
        private float _roughness;
        private ObjectTK.Textures.Texture _roughnessTexture;
       // private PbrShaderProgram _shaderProgram;
        private ObjectTK.Textures.Texture _ssaoMap;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// the albedo texure
        /// </summary>
        public Texture Albedo { get => _albedo; set => _albedo = value; }

        /// <summary>
        /// the basecolor factor
        /// </summary>
        public float BasecolorFactor { get => _basecolorFactor; set => _basecolorFactor = value; }

        /// <summary>
        /// the emissive factor
        /// </summary>
        public float EmissiveFactor { get => _emissiveFactor; set => _emissiveFactor = value; }

        /// <summary>
        /// the emissive texture
        /// </summary>
        public Texture EmissiveMap { get => _emissiveMap; set => _emissiveMap = value; }

        /// <summary>
        /// the metallic factor
        /// </summary>
        public float Metalicness { get => _metalicness; set => _metalicness = value; }

        /// <summary>
        /// the metallic texture
        /// </summary>
        public Texture MetalTexture { get => _metalTexture; set => _metalTexture = value; }

        /// <summary>
        /// the normal factor
        /// </summary>
        public float NormalFactor { get => _normalFactor; set => _normalFactor = value; }

        /// <summary>
        /// the normalmap texture
        /// </summary>
        public Texture NormalMap { get => _normalMap; set => _normalMap = value; }

        /// <summary>
        /// the roughness factor
        /// </summary>
        public float Roughness { get => _roughness; set => _roughness = value; }

        /// <summary>
        /// the roughness texture
        /// </summary>
        public Texture RoughnessTexture { get => _roughnessTexture; set => _roughnessTexture = value; }

        /// <summary>
        /// the shader programm that should be used
        /// </summary>
       // public PbrShaderProgram ShaderProgram { get => _shaderProgram; set => _shaderProgram = value; }
        /// <summary>
        /// the ssao texture
        /// </summary>
        public Texture SsaoMap { get => _ssaoMap; set => _ssaoMap = value; }


        #endregion Public Properties

        public override void Init()
        {
            if (initialized)
                return;
            _shaderProgram = SXProgramFactory.Create<PbrShaderProgram>();
            _shaderProgram.Use();
            PbrShaderProgram pbrShader = _shaderProgram as PbrShaderProgram;


            uint flag =0;
            if (_albedo != null)
            {
                flag |= (uint)ShaderFlag.ALBEDO;
                pbrShader.Albedo.BindTexture(TextureUnit.Texture0, _albedo);
            }
          
            if (_normalMap != null)
            {
                flag |= (uint)ShaderFlag.NORMAL;
                pbrShader.NormalTex.BindTexture(TextureUnit.Texture1, _normalMap);
                pbrShader.NormalFactor.Set(_normalFactor);
            }
            
            if(_metalTexture!=null){
                flag |= (uint)ShaderFlag.METAL;
                pbrShader.MetalTex.BindTexture(TextureUnit.Texture2,_metalTexture);
                pbrShader.Metalicness.Set(_metalicness);
            }

            if(_roughnessTexture!=null){
                 flag |= (uint)ShaderFlag.ROUGHNESS;
                pbrShader.RoughnessTex.BindTexture(TextureUnit.Texture3,_roughnessTexture);
                pbrShader.Roughness.Set(_roughness);
            }
            if(_emissiveMap!=null){
                  flag |= (uint)ShaderFlag.EMISSIVE;
                pbrShader.EmissiveMap.BindTexture(TextureUnit.Texture4,_emissiveMap);
                pbrShader.EmissiveFactor.Set(_emissiveFactor);
            }

             pbrShader.Flags.Set(flag);
             if(_normalFactor==0)
                _normalFactor=1;
            initialized = true;
        }
        public void Update()
        {

        }

        public override void Use()
        {
            _shaderProgram.Use();
             PbrShaderProgram pbrShader = _shaderProgram as PbrShaderProgram;
            if (_albedo != null)
                _albedo.Bind(TextureUnit.Texture0);
            if (_normalMap != null){
               _normalMap.Bind(TextureUnit.Texture1);
               pbrShader.NormalFactor.Set(_normalFactor);
            }
            if(_metalTexture!=null)
               _metalTexture.Bind(TextureUnit.Texture2);
            if(_roughnessTexture!=null)
              _roughnessTexture.Bind(TextureUnit.Texture3);
            if(_emissiveMap!=null)
              _emissiveMap.Bind(TextureUnit.Texture4);

        }

        public void Release(){
           

        }
    }
}