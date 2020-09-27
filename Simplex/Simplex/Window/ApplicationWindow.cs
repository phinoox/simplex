using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using Simplex.Gui.Renderer;
using Simplex.Rendering;
using Simplex.Scene;
using Simplex.Util;
using System;
using System.Collections.Generic;
using System.IO;

namespace Simplex.Windowing
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
        private GuiRenderer _guiRender = new GuiRenderer();
        private HashSet<Key> keysDown = new HashSet<Key>();
        private Scene3D _scene = new Scene3D();

        private SXRenderer _renderer = new SXRenderer();
        private IGraphicsContext _sceneContext;
        private IGraphicsContext _guiContext;
        

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// creates window with given parameters
        /// defaults to windowed mode
        /// </summary>
        /// <param name="width">the widht of the window</param>
        /// <param name="height">the height of the window</param>
        /// <param name="title">the window title</param>
        public ApplicationWindow(int width, int height, string title) : base(width, height, title, GameWindowFlags.Default, GraphicsMode.Default, DisplayDevice.Default)
        {
            _sceneContext = new GraphicsContext(GraphicsMode.Default, this.WindowInfo, 4, 5, GraphicsContextFlags.Default);
            _sceneContext.MakeCurrent(this.WindowInfo);
             
            _sceneContext.LoadAll();
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            //GL.BlendFuncSeparate(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha,BlendingFactorSrc.One,BlendingFactorDest.OneMinusSrcAlpha);  
  
            GL.BlendFunc(BlendingFactor.One,BlendingFactor.OneMinusSrcAlpha);
            _renderer.Init(width,height);
            _guiContext = new GraphicsContext(GraphicsMode.Default, this.WindowInfo, 4, 5, GraphicsContextFlags.ForwardCompatible);
            _guiContext.MakeCurrent(this.WindowInfo);
            //vg = GlNanoVg.CreateGl(NvgCreateFlags.AntiAlias |
            //     NvgCreateFlags.StencilStrokes |
            //     NvgCreateFlags.Debug);
            _guiRender.Init(width,height);
            if(!File.Exists("Data/Fonts/OpenSans-Regular.ttf"))
               Console.WriteLine("could not finde default font file");
            //int defaultfontLoaded=NanoVg.CreateFont(vg, "sans", "Data/Fonts/OpenSans-Regular.ttf");
            //if(defaultfontLoaded != 0)
            //    Console.WriteLine("could not load default font");
            this.Visible = true;
            Logger.Default.Info(_sceneContext);
            this.KeyDown += ApplicationWindow_KeyDown;
            this.KeyUp += ApplicationWindow_KeyUp;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// the main gui renderer
        /// </summary>
        public GuiRenderer GuiRender { get => _guiRender; set => _guiRender = value; }

        /// <summary>
        /// the main scene
        /// </summary>
        public Scene3D Scene { get => _scene; set => _scene = value; }

        /// <summary>
        /// the created nanovg context that can be used for drawing
        /// </summary>
        public SXRenderer Renderer { get => _renderer; set => _renderer = value; }

        #endregion Public Properties

        #region Private Methods

        private void ApplicationWindow_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            keysDown.Add(e.Key);
        }

        private void ApplicationWindow_KeyUp(object sender, KeyboardKeyEventArgs e)
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
            GuiRender.Mouse_ButtonDown(e);
        }

        /// <summary>
        /// base override,forwards to gui
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _guiRender.MouseEntered();
        }

        /// <summary>
        /// base override,forwards to gui
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _guiRender.MouseLeave();
        }

        /// <summary>
        /// base override, forwards to gui
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            _guiRender.Mouse_Move(e);
        }

        /// <summary>
        /// base override,forwards to gui
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            GuiRender.Mouse_ButtonUp(e);
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
        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            _renderer.ResizeFrameBuffer(Width,Height);
            _guiRender.Resize(Width,Height);
        }

        #endregion Protected Methods

        #region Public Methods

        /// <summary>
        /// returns true if a specific key is held down
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool isKeyDown(Key key)
        {
            return keysDown.Contains(key);
        }

        /// <summary>
        /// renders the gui
        /// </summary>
        /// <param name="delta"></param>
        public void RenderGui(float delta)
        {
            _guiContext.MakeCurrent(this.WindowInfo);
             GL.Viewport(0, 0, Width, Height);
            _guiRender.Render();
            if (!_sceneContext.IsCurrent)
                _sceneContext.MakeCurrent(this.WindowInfo);
        }

        /// <summary>
        /// renders the scene
        /// </summary>
        /// <param name="delta"></param>
        public void RenderScene(float delta)
        {
            if (!_sceneContext.IsCurrent)
                _sceneContext.MakeCurrent(this.WindowInfo);

            this._renderer.Render(_scene);
        }
        /// <summary>
        /// swaps the buffers of the opengl context
        /// </summary>
        public void SwapBuffers()
        {
            _sceneContext.SwapBuffers();
        }

        #endregion Public Methods
    }
}