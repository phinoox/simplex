using Simplex.Core;
using Simplex.Core.Gui;
using Simplex.Core.Loaders;
using Simplex.Core.Rendering;
using Simplex.Core.Scene;
using System.IO;
using OpenTK.Input;
using OpenTK;
using System;

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
        #endregion Private Fields

        #region Private Methods

        private void drawGui()
        {
        }

        private void MainWindow_FileDrop(object sender, OpenTK.Input.FileDropEventArgs e)
        {
            string ext = Path.GetExtension(e.FileName);
            switch (ext)
            {
                case ".gltf":
                case ".glb":
                    {
                        MeshLoader mloader = new MeshLoader();
                        gltfMesh = mloader.LoadMesh(e.FileName);
                        //this.MainWindow.Scene.RootNode.ClearChildren();
                        this.MainWindow.Scene.RootNode.AddChild(gltfMesh);
                        //this.MainWindow.Scene.CurrentCamera.LookAt(node.Translation);
                        break;
                    }
                case ".xaml":
                    {
                        GuiLoader gLoader = new GuiLoader();
                        gLoader.LoadGui(e.FileName);
                        break;
                    }
            }
        }

        private void MainWindow_KeyDown(object sender, KeyboardKeyEventArgs key)
        {

        }

        private void MainWindow_MouseMove(object sender, MouseMoveEventArgs e)
        {
            if (e.Mouse.IsButtonDown(MouseButton.Right))
            {
                if (e.XDelta != 0 || e.YDelta != 0)
                {
                    float sensitivity = 10.0f;
                    Camera cam = MainWindow.Scene.CurrentCamera;
                    float deltaX = e.XDelta * sensitivity;
                    float deltaY = e.YDelta * sensitivity;
                    cam.RotateX(MathHelper.DegreesToRadians(deltaY));
                    cam.RotateY(MathHelper.DegreesToRadians(deltaX));
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
            LightNode directional = new LightNode();
            directional.Name="Sun";
            directional.LightType = LightTypes.DIRECTIONAL;
            directional.Translation = new Vector3(5,7,3);
            directional.LookAt(Vector3.Zero);
            MainWindow.Scene.RootNode.AddChild(directional);
            //ctrl = new Control();
            MainWindow.FileDrop += MainWindow_FileDrop;
            MainWindow.KeyDown += MainWindow_KeyDown;
            MainWindow.MouseMove += MainWindow_MouseMove;
        }

        protected override void onTick(float delta)
        {
            base.onTick(delta);
            if (MainWindow.isKeyDown(Key.Escape))
                this.ShouldClose = true;
            float camSpeedModifier = MainWindow.isKeyDown(Key.ShiftLeft) ? 0.05f : 0.01f;
            float speedRatio = camSpeedModifier * delta;
            Camera cam = MainWindow.Scene.CurrentCamera;
            if (MainWindow.isKeyDown(Key.W))
                cam.Translation = cam.Translation + cam.Forward * speedRatio;
            if (MainWindow.isKeyDown(Key.S))
                cam.Translation = cam.Translation - cam.Forward * speedRatio;
            if (MainWindow.isKeyDown(Key.A))
                cam.Translation = cam.Translation - new Vector3(cam.Right.X,0,cam.Right.Z) * speedRatio;
            if (MainWindow.isKeyDown(Key.D))
                cam.Translation = cam.Translation + new Vector3(cam.Right.X,0,cam.Right.Z) * speedRatio;
            if (MainWindow.isKeyDown(Key.Q))
                cam.Translation = cam.Translation - Vector3.UnitY * speedRatio;
            if (MainWindow.isKeyDown(Key.E))
                cam.Translation = cam.Translation + Vector3.UnitY * speedRatio;

            //cam.RotateY(1.1f*delta);
            time+=delta *0.01f;
           // LightNode directional = MainWindow.Scene.RootNode.FindChild("Sun") as LightNode;
            if(gltfMesh!=null){
                gltfMesh.RotateY(MathHelper.DegreesToRadians(0.25f));
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