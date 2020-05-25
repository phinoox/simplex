using Simplex.Core.Configs;
using Simplex.Core.Window;
using System;
using System.Threading;

namespace Simplex.Core
{
    /// <summary>
    /// This class is the main entry class for an application based on this library
    /// For more convinient usage you can inherit it or simply use the events
    /// </summary>
    public class ApplicationBase
    {
        #region Public Events

        /// <summary>
        /// eventhandler for engine tick events
        /// </summary>
        public event EventHandler TickHandler;

        #endregion Public Events

        #region Private Fields

        private static ApplicationBase instance;
        private ApplicationConfig appConfig;
        private float frameTime = 0;
        private ApplicationWindow mainWindow;
        private bool shouldClose = false;
        private DateTime startTime;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// default constructor
        /// ToDo : change to singleton pattern
        /// </summary>
        public ApplicationBase()
        {
            instance = this;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// the instance of the application
        /// </summary>
        public static ApplicationBase Instance { get => instance; }

        /// <summary>
        /// returns the initially created window
        /// </summary>
        public ApplicationWindow MainWindow { get => mainWindow; }

        /// <summary>
        /// for safe closing of the application set this to true
        /// </summary>
        public bool ShouldClose { get => shouldClose; set => shouldClose = value; }

        #endregion Public Properties

        #region Private Methods

        private void LoadConfigs()
        {
            appConfig = new ApplicationConfig();
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            this.shouldClose = true;
        }

        #endregion Private Methods

        #region Protected Methods

        /// <summary>
        /// gets called on engine initialization after base configs have been loaded
        /// </summary>
        /// <param name="args"></param>
        protected virtual void onInit(string[] args)
        {
        }

        /// <summary>
        /// gets called from the mainloop before rendering
        /// </summary>
        /// <param name="delta"></param>
        protected virtual void onTick(float delta)
        {
        }

        #endregion Protected Methods

        #region Public Methods

        /// <summary>
        ///Initializes the application and loads the configurations
        /// </summary>
        /// <param name="args"> command line arguments</param>
        /// <returns></returns>
        public bool Init(string[] args)
        {
            LoadConfigs();
            if (appConfig.Fps != 0)
                frameTime = 1000.0f / appConfig.Fps;
            mainWindow = new ApplicationWindow(800, 600, "Simplex app");
            mainWindow.Closed += MainWindow_Closed;
            onInit(args);
            return true;
        }
        /// <summary>
        /// starts the main loop
        /// </summary>
        public void Run()
        {
            startTime = DateTime.Now;
            DateTime lastframe = DateTime.Now;
            DateTime lastFps = DateTime.Now;

            int frame = 0;
            int fps = 0;

            while (!shouldClose && mainWindow.Exists)
            {
                DateTime tmp = DateTime.Now;
                int fpsTimePassed = (int)(tmp - lastFps).TotalMilliseconds;
                if (fpsTimePassed > 1000)
                {
                    fps = frame;
                    lastFps = tmp;
                    frame = 0;
                }
                float delta = (float)tmp.Subtract(lastframe).TotalMilliseconds;
                lastframe = tmp;
                int canSleep = (int)(frameTime - delta);
                if (frameTime != 0 && canSleep > 0)
                    Thread.Sleep(canSleep);
                onTick(delta);
                mainWindow.RenderScene(delta);
                mainWindow.GuiRender.DrawText($"FPS:{fps}", 15, 15);
                //mainWindow.GuiRender.DrawText($"Slept:{canSleep}", 60, 15);
                mainWindow.RenderGui(delta);
                mainWindow.SwapBuffers();
                EventHandler handler = TickHandler;
                handler?.Invoke(this, new FrameEventArgs() { deltaTime = delta });
                mainWindow.ProcessEvents();
                frame++;
            }
        }

        #endregion Public Methods
    }

    /// <summary>
    /// Eventargs for the applications tick event
    /// </summary>
    public class FrameEventArgs : EventArgs
    {
        #region Public Fields

        /// <summary>
        /// time in milliseconds since the last frame
        /// </summary>
        public float deltaTime;

        #endregion Public Fields
    }
}