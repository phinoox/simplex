using Simplex;
using Simplex.Gui;
using Simplex.Loaders;
using Simplex.Rendering;
using Simplex.Scene;
using System.IO;
using OpenTK.Input;
using System.Numerics;
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
        private SceneNode dummy;
        private bool up = true;
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
                        dummy.ClearChildren();
                        dummy.AddChild(gltfMesh);
                        this.MainWindow.Scene.CurrentCamera.Translation = gltfMesh.Bounds.Max + new Vector3(0, 3, 3);
                        //this.MainWindow.Scene.CurrentCamera.LookAt(gltfMesh.Bounds.Center);
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
            if(key.Key==Key.F2){
                if(this.MainWindow.Renderer.Mode==RenderMode.SHADED)
                   this.MainWindow.Renderer.Mode=RenderMode.DEBUG;
                else
                    this.MainWindow.Renderer.Mode=RenderMode.SHADED;
            }
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
                    cam.RotateX(OpenTK.MathHelper.DegreesToRadians(deltaY));
                    cam.RotateY(OpenTK.MathHelper.DegreesToRadians(deltaX));
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
            directional.Name = "Sun";
            directional.LightType = LightTypes.DIRECTIONAL;
            directional.Translation = new Vector3(0, 7,0);
            directional.LookAt(Vector3.Zero);
            MainWindow.Scene.RootNode.AddChild(directional);

            dummy = new SceneNode();
            MainWindow.Scene.RootNode.AddChild(dummy);
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
            float camSpeedModifier = MainWindow.isKeyDown(Key.ShiftLeft) ? 5f : 1f;
            float speedRatio = camSpeedModifier * (delta* 0.001f);
            Camera cam = MainWindow.Scene.CurrentCamera;

            MainWindow.GuiRender.DrawText($"Cam Position:{cam.Translation}",5f,30f);
            MainWindow.GuiRender.DrawText($"Cam Forward:{cam.Forward},Right:{cam.Right},Up:{cam.Up}",5f,40f);

            if (MainWindow.isKeyDown(Key.W))
                cam.Translation = cam.Translation + cam.Forward * speedRatio;
            if (MainWindow.isKeyDown(Key.S))
                cam.Translation = cam.Translation - cam.Forward * speedRatio;
            if (MainWindow.isKeyDown(Key.A))
                cam.Translation -= Vector3.Normalize(Vector3.Cross(cam.Forward, Vector3.UnitY)) * speedRatio;
            if (MainWindow.isKeyDown(Key.D))
                cam.Translation += Vector3.Normalize(Vector3.Cross(cam.Forward, Vector3.UnitY)) * speedRatio;
            if (MainWindow.isKeyDown(Key.Q))
                cam.Translation = cam.Translation + Vector3.UnitY * speedRatio;
            if (MainWindow.isKeyDown(Key.E))
                cam.Translation = cam.Translation - Vector3.UnitY * speedRatio;
            if (MainWindow.isKeyDown(Key.R))
            {
                cam.Translation=Vector3.Zero;
                cam.Rotation=new Quaternion();
            }
            if (MainWindow.isKeyDown(Key.L)){
                cam.LookAt(Vector3.Zero);
            }
            //cam.RotateY(1.1f*delta);
            time += delta * 0.01f;
            /*LightNode directional = MainWindow.Scene.RootNode.FindChild("Sun") as LightNode;
            if(directional!=null)
                directional.RotateY(delta*0.001f);*/
            /*if (gltfMesh != null)
            {
                //gltfMesh.RotateY(MathHelper.DegreesToRadians(0.25f));
                if ((int)time % 15 == 0)
                    up = !up;
                if (up){
                    GuiRenderer.DefaultRenderer.DrawText("going up",5,40);
                    gltfMesh.TranslateY(0.2f);
                    gltfMesh.TranslateX(0.2f);
                }
                else{
                    GuiRenderer.DefaultRenderer.DrawText("going down",5,40);
                    gltfMesh.TranslateY(-0.2f);
                    gltfMesh.TranslateX(-0.2f);
                }
            }*/
            //GlobalUniforms.LightDir = (Quaternion.FromAxisAngle(Vector3.UnitY,MathHelper.DegreesToRadians(delta*0.05f)) * GlobalUniforms.LightDir).Normalized();
            //MainWindow.GuiRender.DrawText($"Cam Position:{cam.Translation}",5f,30f);
            //float angle = time % 360;
            //MainWindow.GuiRender.DrawText($"Cam Position:{cam.Translation},angle :{angle}",5f,30f);
            //Quaternion rotDelta =Quaternion.FromAxisAngle(Vector3.UnitY,MathHelper.DegreesToRadians(angle)).Normalized();
            //Vector3 target = cam.Translation + (rotDelta) * cam.Forward;
            //cam.Rotation = rotDelta;//(cam.Rotation * rotDelta).Normalized();
            //cam.LookAt(target);
            //ctrl.Draw();
            Matrix4x4 org = Matrix4x4.Identity;
            OpenTK.Matrix4 conv = new OpenTK.Matrix4(org.M11,org.M12,org.M13,org.M14,
                                                    org.M21,org.M22,org.M23,org.M24,
                                                    org.M31,org.M32,org.M33,org.M34,
                                                    org.M41,org.M42,org.M43,org.M44);

        }

        #endregion Protected Methods
    }
}