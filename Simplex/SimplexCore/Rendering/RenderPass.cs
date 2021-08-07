using System;
using System.Collections.Generic;
using System.Text;
using ObjectTK.Buffers;
using ObjectTK.Textures;
using OpenTK.Graphics.OpenGL4;
using Simplex.Core.Scene;

namespace Simplex.Core.Rendering
{
    public enum ResizePolicies
    {
        Window,
        Fixed
    }

    public enum AttachmentType
    {
        Color,
        Depth,
        Normal,
        Emissive,
        Position
    }


    public abstract class RenderPass
    {
        private RenderMode mode = RenderMode.SHADED;
        private static Dictionary<Type, RenderPass> _renderPasses = new Dictionary<Type, RenderPass>();
        private string _name = "";
        private ClearBufferMask _clearMask = ClearBufferMask.None;
        private bool _enableDepthTest = false;
        private bool _enableBlend = false;
        private Framebuffer _renderBuffer;
        private int _framebufferSize = 2048;
        private int _width;
        private int _height;
        private ResizePolicies _resizePolicy = ResizePolicies.Window;
        private Dictionary<AttachmentType, Type> _inputs = new Dictionary<AttachmentType, Type>();
        private Dictionary<AttachmentType, Texture2D> _outputs = new Dictionary<AttachmentType, Texture2D>();
       
       

        public abstract int SortOrder { get; }


        public string Name { get => _name; set => _name = value; }
        public ClearBufferMask ClearMask { get => _clearMask; set => _clearMask = value; }
        public bool EnableDepthTest { get => _enableDepthTest; set => _enableDepthTest = value; }
        public bool EnableBlend { get => _enableBlend; set => _enableBlend = value; }
        public Framebuffer RenderBuffer { get => _renderBuffer; set => _renderBuffer = value; }
        public int FramebufferSize { get => _framebufferSize; set => _framebufferSize = value; }
        public int Width { get => _width; set => _width = value; }
        public int Height { get => _height; set => _height = value; }
        public ResizePolicies ResizePolicy { get => _resizePolicy; set => _resizePolicy = value; }
        public RenderMode Mode { get => mode; set => mode = value; }
        public static Dictionary<Type, RenderPass> RenderPasses { get => _renderPasses; }
       
       



        public void Render(Scene3D scene)
        {
            _renderBuffer.Bind(FramebufferTarget.Framebuffer);

            GL.Clear(_clearMask);

            if (_enableBlend)
                GL.Enable(EnableCap.Blend);
            else
                GL.Disable(EnableCap.Blend);

            if (_enableDepthTest)
                GL.Enable(EnableCap.DepthTest);
            else
                GL.Disable(EnableCap.DepthTest);

            if (_resizePolicy == ResizePolicies.Fixed)
                GL.Viewport(0, 0, _framebufferSize, _framebufferSize);
            else
                GL.Viewport(0, 0, _width, _height);
           
            OnRender(scene);
            Framebuffer.Unbind(FramebufferTarget.Framebuffer);
        }

        protected virtual void OnRender(Scene3D scene) { }

        public void Init(int width, int height)
        {
            _width = width;
            _height = height;
            _renderBuffer = new Framebuffer();
            _renderPasses[this.GetType()] = this;
            _renderBuffer.Bind(FramebufferTarget.Framebuffer);
           
            OnInit(width, height);
            Framebuffer.Unbind(FramebufferTarget.Framebuffer);

        }

        protected virtual void OnInit(int width, int height)
        {

        }

        public void Resize(int width, int height)
        {
            _width = width;
            _height = height;
            _renderBuffer.Dispose();
            _renderBuffer = new Framebuffer();
            _renderBuffer.Bind(FramebufferTarget.Framebuffer);
            OnResize(width, height);
            Framebuffer.Unbind(FramebufferTarget.Framebuffer);
        }

        protected virtual void OnResize(int width, int height) { }


    }
}
