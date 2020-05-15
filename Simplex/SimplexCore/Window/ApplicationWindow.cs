using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using Simplex.Core.Scene;
using Simplex.Core.Util;

namespace Simplex.Core.Window
{
    public class ApplicationWindow : NativeWindow
    {
        private Scene3D scene;
        private HashSet<Key> keysDown = new HashSet<Key>();
        IGraphicsContext context;

        public ApplicationWindow(int width,int height,string title) : base(width, height,title,GameWindowFlags.Default, GraphicsMode.Default, DisplayDevice.Default)
        {
            context = new GraphicsContext(GraphicsMode.Default, this.WindowInfo,4,5,GraphicsContextFlags.ForwardCompatible);
            context.MakeCurrent(this.WindowInfo);
            this.Visible = true;
            Logger.Default.Info(context);
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

        public void SwapBuffers()
        {
            context.SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
        }
    }
}
