using OpenTK.Mathematics;
using Simplex.Core.Scene;
using System;
using System.Collections.Generic;
using System.Text;


namespace Simplex.Core.Rendering.RenderPasses
{
    class LightVolumePass : RenderPass
    {
        public override int SortOrder => 30;

        protected override void OnRender(Scene3D scene)
        {
            Camera cam = scene.CurrentCamera;
            Matrix4 view = cam.getViewMatrix();
            Matrix4 projection = cam.getProjectionMatrix();
            
            List<LightNode> lights = scene.RootNode.FindChildByType<LightNode>();
            foreach (LightNode light in lights)
            {
                if (light.LightType == LightTypes.DIRECTIONAL)
                {
                    List<MeshNode> meshes = scene.RootNode.FindChildByType<MeshNode>();
                    foreach (MeshNode mesh in meshes)
                    {
                        mesh.Render(AlphaMode.OPAQUE);
                    }
                }
                else
                {
                    light.Render(cam);
                }
            }
            
        }

        protected override void OnResize(int width, int height)
        {
            
        }
    }
}
