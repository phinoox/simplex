using System;
using System.Collections.Generic;
using System.Text;
using ObjectTK;
using ObjectTK.Shaders;
using ObjectTK.Textures;

namespace Simplex.Core.Rendering
{
    public class PbrMaterial
    {
        ObjectTK.Shaders.Program  shaderProgram;
        ObjectTK.Textures.Texture albedo;
        ObjectTK.Textures.Texture metalTexture;
        ObjectTK.Textures.Texture roughnessTexture;
        ObjectTK.Textures.Texture emissiveMap;
        ObjectTK.Textures.Texture normalMap;
        ObjectTK.Textures.Texture ssaoMap;

        float metalicness;
        float roughness;
        float normalFactor;
        float emissiveFactor;
        float basecolorFactor;

        public Program ShaderProgram { get => shaderProgram; set => shaderProgram = value; }
        public Texture Albedo { get => albedo; set => albedo = value; }
        public Texture MetalTexture { get => metalTexture; set => metalTexture = value; }
        public Texture RoughnessTexture { get => roughnessTexture; set => roughnessTexture = value; }
        public Texture EmissiveMap { get => emissiveMap; set => emissiveMap = value; }
        public Texture NormalMap { get => normalMap; set => normalMap = value; }
        public Texture SsaoMap { get => ssaoMap; set => ssaoMap = value; }
        public float Metalicness { get => metalicness; set => metalicness = value; }
        public float Roughness { get => roughness; set => roughness = value; }
        public float NormalFactor { get => normalFactor; set => normalFactor = value; }
        public float EmissiveFactor { get => emissiveFactor; set => emissiveFactor = value; }
        public float BasecolorFactor { get => basecolorFactor; set => basecolorFactor = value; }
    }
}
