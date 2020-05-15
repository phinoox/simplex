using Simplex.Core.Configs;
using Simplex.Core.Window;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Simplex.Core
{
    public class FrameEventArgs : EventArgs
    {
        public float deltaTime;
    }
    /**
     * This class is the main entry class for an application based on this library
     * For more convinient usage you can inherit it or simply use the events
     */
    public class ApplicationBase
    {
        ApplicationConfig appConfig;
        DateTime startTime;
        bool shouldClose = false;
        float frameTime=0;
        ApplicationWindow mainWindow;

        public ApplicationWindow MainWindow { get => mainWindow;  }
        public bool ShouldClose { get => shouldClose; set => shouldClose = value; }

        public event EventHandler TickHandler;

        
        private void LoadConfigs() {
            appConfig = new ApplicationConfig();  
        }

        /**
         * Initializes the application and loads the configurations
         */
        public bool Init(string[] args) {
            LoadConfigs();
            if(appConfig.Fps!=0)
              frameTime = 1000.0f / appConfig.Fps;
            onInit(args);
            return true;

        }

        protected virtual void onInit(string[] args)
        {
           
        }

        /**
         * starts the main loop
         */
        public void Run() {
            mainWindow = new ApplicationWindow(800,600,"Simplex app");
            mainWindow.Closed += MainWindow_Closed;
            startTime = DateTime.Now;
            DateTime lastframe = DateTime.Now;
            

            while (!shouldClose)
            {
                mainWindow.SwapBuffers();
                mainWindow.ProcessEvents();
                DateTime tmp = DateTime.Now;
                float delta = (float)tmp.Subtract(lastframe).TotalMilliseconds;
                if (frameTime != 0 && delta < frameTime)
                    Thread.Sleep((int)(frameTime - delta));
                onTick(delta);
                EventHandler handler = TickHandler;
                handler?.Invoke(this, new FrameEventArgs() {deltaTime=delta });
                
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            this.shouldClose = true;
        }

        protected virtual void onTick(float delta)
        {
            
        }
    }
}
