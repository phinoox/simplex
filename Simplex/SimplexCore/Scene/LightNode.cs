using OpenTK;
using OpenTK.Mathematics;
using Simplex.Core.Components;
using System;

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
        private LightBase _light;
        private float _power;

        private Color4 _lightColor = Color4.White;

        public LightTypes LightType { get => _lightType; set => SetLightType(value); }

        private void SetLightType(LightTypes value)
        {
            if (_lightType == value)
                return;

            if (_light != null)
                _light.Dispose();

            _lightType = value;
            
            switch (value)
            {
                case LightTypes.DIRECTIONAL: _light = new DirectionalLight(this);break;
                case LightTypes.POINT: _light = new PointLight(this);break;
                case LightTypes.SPOT: _light = new SpotLight(this);break;

            }
        }

        public bool CastsShadow { get => _castsShadow; set => _castsShadow = value; }
        public int ShadowMapSize { get => _shadowMapSize; set => _shadowMapSize = value; }
        public int Attentuation { get => _attentuation; set => _attentuation = value; }
        public float Power { get => _power; set => _power = value; }
        public Color4 LightColor { get => _lightColor; set => _lightColor = value; }
        public LightBase Light { get => _light; }

        public void Render(Camera cam)
        {

            Matrix4 translation;
            Matrix4 rotation = Matrix4.CreateFromQuaternion(GlobalRotation);
            Matrix4 scale = Matrix4.CreateScale(GlobalScale);
            Matrix4 view = cam.getViewMatrix();
            Matrix4 projection = cam.getProjectionMatrix();


            if (_lightType == LightTypes.DIRECTIONAL)
            {
                //for directional light we just set the pos of the shadowmap camera to the position 
                //of half the farclip towards the scenecamera forward and movie it up  by it's inverted forward
                float halfClip = cam.FarClip * 0.5f;
                Vector3 clipCenter = cam.Translation + (cam.Forward * halfClip);
                Vector3 pos = clipCenter - (Forward * halfClip);
                translation = Matrix4.CreateTranslation(pos);
                Matrix4 model = translation * rotation * scale;
                _light.Render(model);//directional light renders with its own camera in another pass
            }
            else
            {
                translation = Matrix4.CreateTranslation(GlobalTranslation);
                Matrix4 model = translation * rotation * scale;
                Matrix4 mvp = model * view * projection;
                _light.Render(mvp);
            }
            
           
        }

        public override void LookAt(in Vector3 target)
        {
            base.LookAt(target);
        }
    }

}