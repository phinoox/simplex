using OpenTK;

namespace Simplex.Core.Scene {

    public enum LightTypes{
        DIRECTIONAL,
        SPOT,
        POINT
    }
    
    public class LightNode : SceneNode{
        private LightTypes _lightType = LightTypes.POINT;
        private bool _castsShadow;
        private int _shadowMapSize=1024;
        private int _attentuation;

        private float _power;

        private Color _lightColor = Color.White;

        public LightTypes LightType { get => _lightType; set => _lightType = value; }
        public bool CastsShadow { get => _castsShadow; set => _castsShadow = value; }
        public int ShadowMapSize { get => _shadowMapSize; set => _shadowMapSize = value; }
        public int Attentuation { get => _attentuation; set => _attentuation = value; }
        public float Power { get => _power; set => _power = value; }
        public Color LightColor { get => _lightColor; set => _lightColor = value; }
    }

}