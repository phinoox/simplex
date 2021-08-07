using ObjectTK.Buffers;
using ObjectTK.Textures;
using Simplex.Core.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Simplex.Core.Scene;
using System.Collections.Generic;
using Simplex.Core.Components;
using Simplex.Core.Rendering.Programs;
using System.Reflection;
using System;
using System.Linq;
using Simplex.Core.Rendering.RenderPasses;

namespace Simplex.Core.Rendering
{
    /// <summary>
    /// Mode in which the scene object should be rendered
    /// </summary>
    public enum RenderMode
    {
        /// <summary>
        /// fully shaded
        /// </summary>
        SHADED,
        /// <summary>
        /// no lighting
        /// </summary>
        UNLIT,
        /// <summary>
        /// additional debug info
        /// </summary>
        DEBUG,
        /// <summary>
        /// only wireframe
        /// </summary>
        WIREFRAME
    }



    /// <summary>
    /// main class for rendering a 3D scene...I guess
    /// </summary>
    public class SXRenderer
    {
        private RenderMode mode = RenderMode.SHADED;
        private List<RenderPass> _renderPipeline = new List<RenderPass>();
        private bool _needShaderReload = false;
        private int _width, _height;
        private FinalRenderer _finalRenderer;
        /// <summary>
        /// Mode in which the scene object should be rendered
        /// </summary>
        public RenderMode Mode { get => mode; set { mode = value; foreach (var pass in _renderPipeline) pass.Mode = value; } }
        public bool NeedShaderReload { get => _needShaderReload; set => _needShaderReload = value; }


        #region Public Methods

        /// <summary>
        /// initializes the renderer
        /// should be called after opengl context has been created
        /// </summary>
        /// <param name="width">width of the visual area</param>
        /// <param name="height">height of the visual area</param>
        public void Init(int width, int height)
        {
           
            _width = width;
            _height = height;

            InitPipeline(width,height);
            InitFinalRenderer();
            
        }

        private void InitFinalRenderer()
        {
            if(_finalRenderer==null)
                _finalRenderer = new FinalRenderer();
            ColorDepthPass lastPass = _renderPipeline.Where(pass => pass.GetType().IsSubclassOf(typeof(ColorDepthPass))).Last() as ColorDepthPass;
            _finalRenderer.Init(lastPass.ColorBuffer);
        }

        private void InitPipeline(int width,int height)
        {
            Assembly pipelineAssembly = Assembly.GetAssembly(typeof(RenderPass));
            List<Type> types = new List<Type>(pipelineAssembly.GetTypes());
            List < Type > passTypes = types.Where<Type>(x => x.IsSubclassOf(typeof(RenderPass)) && !x.IsAbstract).ToList();
            foreach(var passType in passTypes)
            {
                RenderPass pass = (RenderPass)Activator.CreateInstance(passType);
                _renderPipeline.Add(pass);
            }
            _renderPipeline = _renderPipeline.OrderBy(pass => pass.SortOrder).ToList();

            // initializing passes after list has been ordered cause of dependencies
            // if something crashes here the order is wrong
            foreach(var pass in _renderPipeline)
            {
                pass.Init(width, height);
            }

        }

        /// <summary>
        /// renders the scene
        /// </summary>
        /// <param name="scene">the scene to be rendered</param>
        public void Render(Scene3D scene)
        {
            if (_needShaderReload)
            {
                SXProgramFactory.ReloadShaders();
                _needShaderReload = false;
                ResizeFrameBuffer(_width, _height);
                return;
            }
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            foreach (RenderPass pass in _renderPipeline)
            {
                pass.Render(scene);
            }
            RenderFinalQuad();        
        }

        private void RenderFinalQuad()
        {
            _finalRenderer.Render();   
        }



        /// <summary>
        /// needs to be called whenever the viewport has been resized
        /// </summary>
        /// <param name="width">new widht of the viewport</param>
        /// <param name="height">new height of the viewport</param>
        public void ResizeFrameBuffer(int width, int height)
        {
            if (width == 0 || height == 0)
                return;
            foreach(var pass in _renderPipeline)
            {
                pass.Resize(width, height);
            }
            InitFinalRenderer();
        }

        #endregion Public Methods
    }
}