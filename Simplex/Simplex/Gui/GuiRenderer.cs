using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mime;
using System.Text;
using System.Threading;
using Simplex.Gui.Control;
using Simplex.Gui.Renderer;
using Simplex.Gui.Input;
using Simplex.Platform;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using Simplex.Gui.Skin;
using Simplex.Gui;

namespace Simplex.Gui.Renderer
{
    /// <summary>
    /// Demonstrates the GameWindow class.
    /// </summary>
    public class GuiRenderer : IDisposable
    {

        private OpenTKBase m_Renderer;
        private SkinBase m_Skin;
        private Canvas m_Canvas;
        private InputSystem m_Input;
		private List<string> defaultFonts = new List<string>();
		private int _currentFont=0;
        const int FpsFrames = 50;
        private readonly List<long> m_Ftime;
        private readonly Stopwatch m_Stopwatch;
        private long m_LastTime;
        private float m_TotalTime = 0f;

        private bool m_AltDown = false;

        public GuiRenderer()
        //: base(1024, 768)
        {
            m_Ftime = new List<long>(FpsFrames);
            m_Stopwatch = new Stopwatch();
        }

        public void Keyboard_KeyPress(KeyPressEventArgs e)
        {
            if(e.KeyChar=='f'){
				_currentFont = (_currentFont+1)%defaultFonts.Count;
				//m_Skin.DefaultFont= new Font(m_Renderer, defaultFonts[_currentFont],12);
				m_Skin.SetDefaultFont(defaultFonts[_currentFont],12);
				m_Canvas.Redraw();
			}
			else if (e.KeyChar=='c'){
				m_Skin.Colors.ModalBackground = new Simplex.Gui.Color(200,200,200,275);
			}
        }

        public void Dispose()
        {
            if (m_Canvas != null)
            {
                m_Canvas.Dispose();
                m_Canvas = null;
            }
            if (m_Skin != null)
            {
                m_Skin.Dispose();
                m_Skin = null;
            }
            if (m_Renderer != null)
            {
                m_Renderer.Dispose();
                m_Renderer = null;
            }
            
        }

        /// <summary>
        /// Occurs when a key is pressed.
        /// </summary>
        /// <param name="sender">The KeyboardDevice which generated this event.</param>
        /// <param name="e">The key that was pressed.</param>
        public void Keyboard_KeyDown(KeyboardKeyEventArgs e)
        {
            if (e.Key == global::OpenTK.Input.Key.AltLeft){
                m_AltDown = true;
			}
           
            m_Input.ProcessKeyDown(e);
        }

        public void Keyboard_KeyUp(KeyboardKeyEventArgs e)
        {
            m_AltDown = false;
            m_Input.ProcessKeyUp(e);
        }

        public void Mouse_ButtonDown( MouseButtonEventArgs args)
        {
            m_Input.ProcessMouseMessage(args);
        }

        public void Mouse_ButtonUp(MouseButtonEventArgs args)
        {
            m_Input.ProcessMouseMessage(args);
        }

        public void Mouse_Move( MouseMoveEventArgs args)
        {
            m_Input.ProcessMouseMessage(args);
        }

        public void Mouse_Wheel(MouseWheelEventArgs args)
        {
            m_Input.ProcessMouseMessage(args);
        }

        private void GetInstalledFontCollection()
        {
            
            System.Drawing.Text.InstalledFontCollection ifc = new System.Drawing.Text.InstalledFontCollection();
            System.Drawing.FontFamily[] ff = ifc.Families;

            foreach (System.Drawing.FontFamily family in ff)
            {
				string[] nameParts = family.Name.Split(' ');
				if(nameParts.Length>2)
					continue;
                if (family.IsStyleAvailable(System.Drawing.FontStyle.Regular))
                {
                    System.Drawing.Font f = new System.Drawing.Font(family.Name, 12);
                    defaultFonts.Add(f.Name);
                    f.Dispose();
                }
            }
			
            
        }

        /// <summary>
        /// Setup OpenGL and load resources here.
        /// </summary>
        /// <param name="e">Not used.</param>
        public void Init(int width,int height)
        {
            PlatformBase.Init(new NetCore());
            m_Renderer = new Simplex.Gui.Renderer.OpenTKGL40();
            m_Renderer.Resize(width,height);
            m_Skin = new Simplex.Gui.Skin.TexturedBase(m_Renderer, "Data\\Gui\\DefaultSkin2.png");
			GetInstalledFontCollection();
            m_Skin.DefaultFont = new Font(m_Renderer, "MonoSpace",12);
            m_Canvas = new Canvas(m_Skin);
            m_Input = new InputSystem();
            m_Input.Initialize(m_Canvas);
            TextBox textBox = new TextBox(m_Canvas);
            textBox.Text="hage";
            textBox.ShouldDrawBackground=false;
            textBox.Margin = new Margin(5,5,0,0);
            textBox.TextColor = Color.Black;
            m_Canvas.SetSize(width, height);
            m_Canvas.ShouldDrawBackground = true;
            m_Canvas.BackgroundColor = new Color(200,200,200,150);// m_Skin.Colors.ModalBackground;

            if (Configuration.RunningOnMacOS)
                m_Canvas.Scale = 1.5f;

            m_Stopwatch.Restart();
            m_LastTime = 0;
        }

        internal void MouseEntered()
        {
            
        }

        internal void MouseLeave()
        {
        }

        /// <summary>
        /// Respond to resize events here.
        /// </summary>
        /// <param name="e">Contains information on the new GameWindow size.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        public void Resize(int width,int height)
        {
            m_Renderer.Resize(width, height);

            m_Canvas.SetSize(width, height);
        }

        
        /// <summary>
        /// Add your game rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        public void Render()
        {
           // GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            m_Stopwatch.Start();
            m_Canvas.RenderCanvas();
            m_Stopwatch.Stop();
            int tts = (int)(1000 / 30 - m_Stopwatch.ElapsedMilliseconds); //30 - требуемое кол-во fps
            if (tts > 0)
                Thread.Sleep(tts);
            m_Stopwatch.Restart();
            
        }

        
    }
}
