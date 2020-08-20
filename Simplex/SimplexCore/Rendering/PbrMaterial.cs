using System;
using OpenTK.Graphics.OpenGL;
using ObjectTK.Shaders;
using ObjectTK.Textures;

namespace Simplex.Core.Rendering
{
    /// <summary>
    /// basic class for pbr material
    /// tries to be compatible with gltf materials
    /// </summary>
    public class PbrMaterial : IDisposable
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
        private PbrShaderProgram _shaderProgram;
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
        public PbrShaderProgram ShaderProgram { get => _shaderProgram; set => _shaderProgram = value; }
        /// <summary>
        /// the ssao texture
        /// </summary>
        public Texture SsaoMap { get => _ssaoMap; set => _ssaoMap = value; }

        public void Dispose()
        {
            if (ShaderProgram != null)
                ShaderProgram.Dispose();
        }

        #endregion Public Properties

        public void Init()
        {
            if (initialized)
                return;
            _shaderProgram = SXProgramFactory.Create<PbrShaderProgram>();
            _shaderProgram.Use();
            if (_albedo != null)
            {
                _shaderProgram.HasAlbedo.Set(true);
                _shaderProgram.Albedo.BindTexture(TextureUnit.Texture0, _albedo);
            }
            else
            {
                _shaderProgram.HasAlbedo.Set(false);
            }
            if (_normalMap != null)
            {
                _shaderProgram.NormalTex.BindTexture(TextureUnit.Texture1, _normalMap);
                _shaderProgram.NormalFactor.Set(_normalFactor);
            }
            else
            {
                _shaderProgram.NormalFactor.Set(0);
            }
            initialized = true;
        }
        public void Update()
        {

        }

        public void Use()
        {
            _shaderProgram.Use();
            if (_albedo != null)
                _albedo.Bind(TextureUnit.Texture0);
            if (_normalMap != null)
               _normalMap.Bind(TextureUnit.Texture1);

        }

        public void Release(){
           

        }
    }
}