using NanoVGDotNet.NanoVG;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Simplex.Core.Gui;
using Simplex.Core.Rendering;
using Simplex.Core.Scene;
using Simplex.Core.Util;
using System;
using System.Collections.Generic;
using System.IO;

namespace Simplex.Core.Window
{
    /// <summary>
    /// implementation from opentks native window
    /// every window holds its own 3d scene and guirenderer
    /// handles input
    /// </summary>
    public class ApplicationWindow : NativeWindow
    {
        #region Private Fields

        private bool canUpdate = false;
        private GuiRenderer guiRender = new GuiRenderer();
        private HashSet<Keys> keysDown = new HashSet<Keys>();
        private Scene3D _scene = new Scene3D();

        private SXRenderer _renderer = new SXRenderer();
        private IGraphicsContext sceneContext;
        private IGraphicsContext guiContext;
        private NvgContext vg;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// creates window with given parameters
        /// defaults to windowed mode
        /// </summary>
        /// <param name="width">the widht of the window</param>
        /// <param name="height">the height of the window</param>
        /// <param name="title">the window title</param>
        public ApplicationWindow(int width, int height, string title) : base(new NativeWindowSettings() { Title = title,Size=new OpenTK.Mathematics.Vector2i(width,height) })
        {
            CreateContext();
            sceneContext.MakeCurrent();
             
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            //GL.BlendFuncSeparate(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha,BlendingFactorSrc.One,BlendingFactorDest.OneMinusSrcAlpha);  
  
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            _renderer.Init(width,height);
            _scene.Init();
            //guiContext = new GraphicsContext(GraphicsMode.Default, this.WindowInfo, 4, 5, GraphicsContextFlags.ForwardCompatible);
            guiContext.MakeCurrent();
            vg = GlNanoVg.CreateGl(NvgCreateFlags.AntiAlias |
                 NvgCreateFlags.StencilStrokes |
                 NvgCreateFlags.Debug);
            if(!File.Exists("Data/Fonts/OpenSans-Regular.ttf"))
               Console.WriteLine("could not finde default font file");
            int defaultfontLoaded=NanoVg.CreateFont(vg, "sans", "Data/Fonts/OpenSans-Regular.ttf");
            if(defaultfontLoaded != 0)
                Console.WriteLine("could not load default font");
            this.IsVisible = true;
            Logger.Default.Info(sceneContext);
            this.KeyDown += ApplicationWindow_KeyDown;
            this.KeyUp += ApplicationWindow_KeyUp;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// the main gui renderer
        /// </summary>
        public GuiRenderer GuiRender { get => guiRender; set => guiRender = value; }

        /// <summary>
        /// the main scene
        /// </summary>
        public Scene3D Scene { get => _scene; set => _scene = value; }

        /// <summary>
        /// the created nanovg context that can be used for drawing
        /// </summary>
        public NvgContext Vg { get => vg; set => vg = value; }
        public SXRenderer Renderer { get => _renderer; set => _renderer = value; }

        #endregion Public Properties

        #region Private Methods


        private unsafe void CreateContext()
        {
            sceneContext = new GLFWGraphicsContext(this.WindowPtr);
            guiContext = new GLFWGraphicsContext(this.WindowPtr);
        }

        private void ApplicationWindow_KeyDown(KeyboardKeyEventArgs e)
        {
            keysDown.Add(e.Key);
        }

        private void ApplicationWindow_KeyUp( KeyboardKeyEventArgs e)
        {
            keysDown.Remove(e.Key);
        }

        #endregion Private Methods

        #region Protected Methods

        /// <summary>
        /// base override,forwards to gui
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            GuiRender.MouseDown((int)this.MousePosition.X, (int)this.MousePosition.Y);
        }

        /// <summary>
        /// base override,forwards to gui
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter()
        {
            base.OnMouseEnter();
            guiRender.MouseEntered();
        }

        /// <summary>
        /// base override,forwards to gui
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave()
        {
            base.OnMouseLeave();
            guiRender.MouseLeave();
        }

        /// <summary>
        /// base override, forwards to gui
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            guiRender.MouseMove((int)e.Position.X, (int)e.Position.Y);
        }

        /// <summary>
        /// base override,forwards to gui
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            GuiRender.MouseUp((int)this.MousePosition.X, (int)this.MousePosition.Y);
        }

        /// <summary>
        /// base override,forwards to gui
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
        }

        /// <summary>
        /// base override,forwards to gui and resizes viewport
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            this.Size = new OpenTK.Mathematics.Vector2i(e.Width, e.Height);
            _renderer.ResizeFrameBuffer(e.Width,e.Height);
        }

        #endregion Protected Methods

        #region Public Methods

        /// <summary>
        /// returns true if a specific key is held down
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool isKeyDown(Keys key)
        {
            return keysDown.Contains(key);
        }

        /// <summary>
        /// renders the gui
        /// </summary>
        /// <param name="delta"></param>
        public void RenderGui(float delta)
        {
            guiContext.MakeCurrent();
             GL.Viewport(0, 0, Size.X, Size.Y);
            NanoVg.BeginFrame(vg, Size.X, Size.Y, 1);
            guiRender.Render();
            NanoVg.EndFrame(vg);
            if (!sceneContext.IsCurrent)
                sceneContext.MakeCurrent();
        }

        /// <summary>
        /// renders the scene
        /// </summary>
        /// <param name="delta"></param>
        public void RenderScene(float delta)
        {
            if (!sceneContext.IsCurrent)
                sceneContext.MakeCurrent();

            this._renderer.Render(_scene);
        }
        /// <summary>
        /// swaps the buffers of the opengl context
        /// </summary>
        public void SwapBuffers()
        {
            sceneContext.SwapBuffers();
        }

        #endregion Public Methods
    }
}