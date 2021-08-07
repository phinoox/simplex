using Simplex.Core.Scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Simplex.Core.Rendering.RenderPasses
{
    public class PostProcessPass : RenderPass
    {
        public override int SortOrder => 0;
        private List<PostProcess> _postProcesses = new List<PostProcess>();

        protected override void OnInit(int width, int height)
        {
            base.OnInit(width, height);
            Assembly pipelineAssembly = Assembly.GetAssembly(typeof(PostProcess));
            List<Type> types = new List<Type>(pipelineAssembly.GetTypes());
            List<Type> passTypes = types.Where<Type>(x => x.IsSubclassOf(typeof(PostProcess))).ToList();
            foreach (var passType in passTypes)
            {
                PostProcess pass = (PostProcess)Activator.CreateInstance(passType);
                _postProcesses.Add(pass);
            }
            _postProcesses = _postProcesses.OrderBy(pass => pass.SortOrder).ToList();
        }

        protected override void OnRender(Scene3D scene)
        {
            
        }

        protected override void OnResize(int width, int height)
        {
            
        }
    }
}
