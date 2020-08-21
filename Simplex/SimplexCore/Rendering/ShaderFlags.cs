namespace Simplex.Core.Rendering {

    public enum ShaderFlag{
        ALBEDO = 1 << 0,
        NORMAL = 1 << 1,
        METAL = 1 << 2,
        ROUGHNESS = 1 << 3,
        EMISSIVE = 1 << 4
    }
}