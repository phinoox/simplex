using ObjectTK.Shaders;
using ObjectTK.Textures;

namespace Simplex.Core.Rendering
{
    /// <summary>
    /// basic class for pbr material
    /// tries to be compatible with gltf materials
    /// </summary>
    public class PbrMaterial
    {
        #region Private Fields

        private ObjectTK.Textures.Texture albedo;
        private float basecolorFactor;
        private float emissiveFactor;
        private ObjectTK.Textures.Texture emissiveMap;
        private float metalicness;
        private ObjectTK.Textures.Texture metalTexture;
        private float normalFactor;
        private ObjectTK.Textures.Texture normalMap;
        private float roughness;
        private ObjectTK.Textures.Texture roughnessTexture;
        private ObjectTK.Shaders.Program shaderProgram;
        private ObjectTK.Textures.Texture ssaoMap;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// the albedo texure
        /// </summary>
        public Texture Albedo { get => albedo; set => albedo = value; }

        /// <summary>
        /// the basecolor factor
        /// </summary>
        public float BasecolorFactor { get => basecolorFactor; set => basecolorFactor = value; }

        /// <summary>
        /// the emissive factor
        /// </summary>
        public float EmissiveFactor { get => emissiveFactor; set => emissiveFactor = value; }

        /// <summary>
        /// the emissive texture
        /// </summary>
        public Texture EmissiveMap { get => emissiveMap; set => emissiveMap = value; }

        /// <summary>
        /// the metallic factor
        /// </summary>
        public float Metalicness { get => metalicness; set => metalicness = value; }

        /// <summary>
        /// the metallic texture
        /// </summary>
        public Texture MetalTexture { get => metalTexture; set => metalTexture = value; }

        /// <summary>
        /// the normal factor
        /// </summary>
        public float NormalFactor { get => normalFactor; set => normalFactor = value; }

        /// <summary>
        /// the normalmap texture
        /// </summary>
        public Texture NormalMap { get => normalMap; set => normalMap = value; }

        /// <summary>
        /// the roughness factor
        /// </summary>
        public float Roughness { get => roughness; set => roughness = value; }

        /// <summary>
        /// the roughness texture
        /// </summary>
        public Texture RoughnessTexture { get => roughnessTexture; set => roughnessTexture = value; }

        /// <summary>
        /// the shader programm that should be used
        /// </summary>
        public Program ShaderProgram { get => shaderProgram; set => shaderProgram = value; }
        /// <summary>
        /// the ssao texture
        /// </summary>
        public Texture SsaoMap { get => ssaoMap; set => ssaoMap = value; }

        #endregion Public Properties
    }
}