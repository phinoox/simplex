using Simplex.Core;
using Simplex.Core.Gui;
using Simplex.Core.Loaders;
using Simplex.Core.Rendering;
using Simplex.Core.Scene;
using System.IO;
using OpenTK.Input;
using OpenTK;
using OpenTK.Mathematics;
using System;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Simplex.Core.Components;

namespace Simplex.Editor
{
    internal class EditorApplication : ApplicationBase
    {
        #region Private Fields
        private float time;
        private Control ctrl;
        private string guiPath;
        private string scenePath;
        private SceneNode gltfMesh;
        private SceneNode dummy;
        private bool up = true;
        private float moveStep = 0.025f;
        SceneNode observed;
        #endregion Private Fields
        SceneNode _selected = null;
        DirectionalLight _directionalLight;

        #region Private Methods

        private void drawGui()
        {
        }

        private void MainWindow_FileDrop(FileDropEventArgs e)
        {
            string file = e.FileNames[0];
            string ext = Path.GetExtension(file);
            switch (ext)
            {
                case ".gltf":
                case ".glb":
                    {
                        MeshLoader mloader = new MeshLoader();
                        gltfMesh = mloader.LoadMesh(file);
                        gltfMesh.Name = Path.GetFileNameWithoutExtension(file);
                        //dummy.ClearChildren();
                        dummy.AddChild(gltfMesh);
                        // this.MainWindow.Scene.CurrentCamera.Translation = gltfMesh.Bounds.Max + new Vector3(0, 3, 3);
                        //this.MainWindow.Scene.CurrentCamera.LookAt(gltfMesh.Bounds.Center);
                        break;
                    }
                case ".xaml":
                    {
                        GuiLoader gLoader = new GuiLoader();
                        gLoader.LoadGui(file);
                        break;
                    }
            }
        }

        private void MainWindow_KeyDown(KeyboardKeyEventArgs key)
        {
            if (key.Key == Keys.F2)
            {
                if (this.MainWindow.Renderer.Mode == RenderMode.SHADED)
                    this.MainWindow.Renderer.Mode = RenderMode.DEBUG;
                else
                    this.MainWindow.Renderer.Mode = RenderMode.SHADED;
            }
            else if (key.Key == Keys.Tab && gltfMesh != null)
            {
                if (observed == MainWindow.Scene.CurrentCamera)
                    observed = dummy;
                else
                    observed = MainWindow.Scene.CurrentCamera;
            }
            else if (MainWindow.isKeyDown(Keys.R))
            {
                observed.Translation = Vector3.Zero;
                observed.Rotation = Quaternion.Identity;
            }
            else if (MainWindow.isKeyDown(Keys.L))
            {
                MainWindow.Scene.CurrentCamera.LookAt(Vector3.Zero);
            }
        }

        private void MainWindow_MouseMove(MouseMoveEventArgs e)
        {
            MouseState mouseState = this.MainWindow.MouseState;
            if (mouseState.IsButtonDown(MouseButton.Right))
            {
                if (e.DeltaX != 0 || e.DeltaY != 0)
                {
                    float sensitivity = 5.75f;
                    Camera cam = MainWindow.Scene.CurrentCamera;
                    float deltaX = e.DeltaX * sensitivity;
                    float deltaY = e.DeltaY * sensitivity;
                    if (deltaY != 0)
                        cam.Pitch(MathHelper.DegreesToRadians(-deltaY));
                    if (deltaX != 0)
                        cam.Yaw(MathHelper.DegreesToRadians(deltaX));

                    //Quaternion rotY = Quaternion.FromAxisAngle(cam.Up, MathHelper.DegreesToRadians(deltaX * 2f)).Normalized();
                    //Quaternion rotX = Quaternion.FromAxisAngle(cam.Right, MathHelper.DegreesToRadians(deltaY * 2f)).Normalized();
                    //Quaternion finalRot = (rotY * rotX).Normalized();
                    //cam.Rotation *= finalRot ;
                    //Console.WriteLine($"target :{target},deltax:{deltaX},deltaY :{deltaY}");
                    //cam.LookAt(finalRot * cam.Forward);
                    //cam.Rotation = rotX * rotY;
                }
            }
        }

        #endregion Private Methods

        #region Protected Methods

        protected override void onInit(string[] args)
        {
            base.onInit(args);
            //MainWindow.Scene.CurrentCamera.Translation = new OpenTK.Vector3( 8, 3, 0);
            //MainWindow.Scene.CurrentCamera.LookAt(new OpenTK.Vector3(0));
            string sponza = "Data\\sponza\\Sponza.gltf";
            LightNode directional = MainWindow.Scene.DirectionalLight;
            directional.Name = "Sun";
            directional.LightType = LightTypes.DIRECTIONAL;
            //directional.LookAt(new Vector3(1, -6, 1));
            directional.Pitch(-80);
            directional.Yaw(30);
            _directionalLight = directional.Light as DirectionalLight;
            

            dummy = new SceneNode();
            MainWindow.Scene.RootNode.AddChild(dummy);
            //ctrl = new Control();
            MainWindow.FileDrop += MainWindow_FileDrop;
            MainWindow.KeyDown += MainWindow_KeyDown;
            MainWindow.MouseMove += MainWindow_MouseMove;
            MainWindow.MouseDown += MainWindow_MouseDown;
            MainWindow.MouseUp += MainWindow_MouseUp;
            Camera cam = MainWindow.Scene.CurrentCamera;
            cam.Translation = new Vector3(0, 0, 0);
            observed = cam;
        }

        private void MainWindow_MouseUp(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Right)
            {
                MainWindow.CursorGrabbed = false;
                MainWindow.CursorVisible = true;
            }
            
        }

        private void MainWindow_MouseDown(MouseButtonEventArgs e)
        {
       
            if (e.Button == MouseButton.Right)
                MainWindow.CursorGrabbed = true;
            else if (e.Button == MouseButton.Left)
                PickObject();
        }

        private void PickObject()
        {
            
        }

        protected override void onTick(float delta)
        {
            base.onTick(delta);
            if (MainWindow.isKeyDown(Keys.Escape))
                this.ShouldClose = true;
            float camSpeedModifier = MainWindow.isKeyDown(Keys.LeftShift) ? 5f : 1f;
            float speedRatio = camSpeedModifier * (delta * 0.002f);
            Camera cam = MainWindow.Scene.CurrentCamera;

            // if (gltfMesh != null)
            //   observed = gltfMesh;

            MainWindow.GuiRender.DrawText($"Selected:{observed.Name}", 5f, 30f);
            MainWindow.GuiRender.DrawText($" Position:{observed.Translation}", 5f, 40f);
            MainWindow.GuiRender.DrawText($" Forward:{observed.Forward},Right:{observed.Right},Up:{observed.Up}", 5f, 50f);


            if (MainWindow.isKeyDown(Keys.W))
                observed.Translation = observed.Translation + observed.Forward * speedRatio;
            if (MainWindow.isKeyDown(Keys.S))
                observed.Translation = observed.Translation - observed.Forward * speedRatio;
            if (MainWindow.isKeyDown(Keys.Z))
                observed.Pitch(0.05f * speedRatio);
            if (MainWindow.isKeyDown(Keys.X))
                observed.Pitch(-0.05f * speedRatio);
            if (MainWindow.isKeyDown(Keys.C))
                observed.Yaw(0.05f * speedRatio, false);
            if (MainWindow.isKeyDown(Keys.V))
                observed.Yaw(-0.05f * speedRatio, false);
            if (MainWindow.isKeyDown(Keys.B))
                observed.Roll(0.05f * speedRatio);
            if (MainWindow.isKeyDown(Keys.N))
                observed.Roll(-0.05f * speedRatio);

            if (MainWindow.isKeyDown(Keys.A))
                observed.Translation -= Vector3.Normalize(Vector3.Cross(observed.Forward, WorldDefaults.Up)) * speedRatio;
            if (MainWindow.isKeyDown(Keys.D))
                observed.Translation += Vector3.Normalize(Vector3.Cross(observed.Forward, WorldDefaults.Up)) * speedRatio;
            if (MainWindow.isKeyDown(Keys.Q))
                observed.Translation = observed.Translation + WorldDefaults.Up * speedRatio;
            if (MainWindow.isKeyDown(Keys.E))
                observed.Translation = observed.Translation - WorldDefaults.Up * speedRatio;



            //cam.RotateY(1.1f*delta);
            time += delta * 0.01f;
            /*LightNode directional = MainWindow.Scene.RootNode.FindChild("Sun") as LightNode;
            if(directional!=null)
                directional.RotateY(delta*0.001f);*/
            //cam.RotateY(moveStep);
            _directionalLight.Parent.Yaw(delta * 0.01f,false);

            if (gltfMesh != null && false)
            {
                //gltfMesh.RotateY(MathHelper.DegreesToRadians(0.25f));
                if ((int)time > 50)
                {
                    // up = !up;
                    time = 0;
                }

                if (up)
                {
                    //GuiRenderer.DefaultRenderer.DrawText("going up",5,40);
                    gltfMesh.Yaw(moveStep);
                    //gltfMesh.TranslateX(moveStep);
                }
                else
                {
                    // GuiRenderer.DefaultRenderer.DrawText("going down",5,40);
                    gltfMesh.Yaw(-moveStep);
                    // gltfMesh.TranslateX(-moveStep);
                }
            }
            //GlobalUniforms.LightDir = (Quaternion.FromAxisAngle(Vector3.UnitY,MathHelper.DegreesToRadians(delta*0.05f)) * GlobalUniforms.LightDir).Normalized();
            //MainWindow.GuiRender.DrawText($"Cam Position:{cam.Translation}",5f,30f);
            //float angle = time % 360;
            //MainWindow.GuiRender.DrawText($"Cam Position:{cam.Translation},angle :{angle}",5f,30f);
            //Quaternion rotDelta =Quaternion.FromAxisAngle(Vector3.UnitY,MathHelper.DegreesToRadians(angle)).Normalized();
            //Vector3 target = cam.Translation + (rotDelta) * cam.Forward;
            //cam.Rotation = rotDelta;//(cam.Rotation * rotDelta).Normalized();
            //cam.LookAt(target);
            //ctrl.Draw();
        }

        #endregion Protected Methods
    }
}