using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NanoVGDotNet.NanoVG;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using Simplex.Core.Gui;
using Simplex.Core.Scene;
using Simplex.Core.Util;

namespace Simplex.Core.Window
{
    public class ApplicationWindow : NativeWindow
    {
        private Scene3D scene =new Scene3D();
        private GuiWindow guiWindow = new GuiWindow();
        private HashSet<Key> keysDown = new HashSet<Key>();
        IGraphicsContext sceneContext;
        NvgContext vg;
        
        bool canUpdate = false;
        public NvgContext Vg { get => vg; set => vg = value; }
        public Scene3D Scene { get => scene; set => scene = value; }

        public ApplicationWindow(int width,int height,string title) : base(width, height,title,GameWindowFlags.Default, GraphicsMode.Default, DisplayDevice.Default)
        {
            sceneContext = new GraphicsContext(GraphicsMode.Default, this.WindowInfo,4,5,GraphicsContextFlags.ForwardCompatible);
            sceneContext.MakeCurrent(this.WindowInfo);
            sceneContext.LoadAll();
            vg = GlNanoVg.CreateGl(NvgCreateFlags.AntiAlias |
                 NvgCreateFlags.StencilStrokes |
                 NvgCreateFlags.Debug);
            NanoVg.CreateFont(vg, "sans", "Fonts/OpenSans-Regular.ttf");


            this.Visible = true;
            Logger.Default.Info(sceneContext);
            this.KeyDown += ApplicationWindow_KeyDown;
            this.KeyUp += ApplicationWindow_KeyUp;
        }

        private void ApplicationWindow_KeyUp(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            keysDown.Add(e.Key);
        }

        private void ApplicationWindow_KeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            keysDown.Remove(e.Key);
        }

        public bool isKeyDown(Key key)
        {
            return keysDown.Contains(key);
        }

        public void RenderScene(float delta)
        {
            if(!sceneContext.IsCurrent)
               sceneContext.MakeCurrent(this.WindowInfo);
            
            this.scene.Render(delta);
            
            
        }

        public void RenderGui(float delta)
        {
            //guiContext.MakeCurrent(this.WindowInfo);
            NanoVg.BeginFrame(vg, Width, Height, 1);
            guiWindow.Render();
            NanoVg.EndFrame(vg);
          
        }

        

        public void SwapBuffers()
        {
            sceneContext.SwapBuffers();   
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
        }
    }
}
