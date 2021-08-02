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
        private bool _castsShadow = true;
        private int _shadowMapSize=1024;
        private int _attentuation = 100;
        private LightBase _light;
        private float _power = 1.0f;

        private Color4 _lightColor = Color4.White;

        /// <summary>
        /// the internal type of the light
        /// </summary>
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
                case LightTypes.DIRECTIONAL: _light = new DirectionalLight();break;
                case LightTypes.POINT: _light = new PointLight();break;
                case LightTypes.SPOT: _light = new SpotLight();break;

            }
            _light.Parent = this;
        }

        /// <summary>
        /// determines if the light casts shadows
        /// </summary>
        public bool CastsShadow { get => _castsShadow; set => _castsShadow = value; }
        /// <summary>
        /// the size of the shadowmap, default is 1024
        /// </summary>
        public int ShadowMapSize { get => _shadowMapSize; set => _shadowMapSize = value; }
        /// <summary>
        /// the attentuation for the light,defaults to 100
        /// </summary>
        public int Attentuation { get => _attentuation; set => _attentuation = value; }
        /// <summary>
        /// the power of the light, defaults to 1
        /// </summary>
        public float Power { get => _power; set => _power = value; }
        /// <summary>
        /// the main color of the light, defaults to white
        /// </summary>
        public Color4 LightColor { get => _lightColor; set => _lightColor = value; }
        /// <summary>
        /// the nested light component
        /// </summary>
        public LightBase Light { get => _light; }

        /// <summary>
        /// renders the light for the given camera
        /// </summary>
        /// <param name="cam"></param>
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


    }

}