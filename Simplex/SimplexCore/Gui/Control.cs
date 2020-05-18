using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using NanoVGDotNet.NanoVG;
using NanoVGDotNet;
using Simplex.Core.Util;

namespace Simplex.Core.Gui
{
    public class Control
    {
        SimplexColor backgroundColor=new SimplexColor(OpenTK.Color.Orange);
        SimplexColor textColor = new SimplexColor(OpenTK.Color.Black);
        SimplexColor borderColor = new SimplexColor(OpenTK.Color.Black);
        protected OpenTK.Vector2 position= new OpenTK.Vector2();
        OpenTK.Vector2 size = new OpenTK.Vector2();
        float borderRadius = 0;
        float borderThickness = 0;
        bool scissorContents = true;
        string name;
        bool enabled = true;
        bool focused = false;
        protected List<Control> children = new List<Control>();

        public float Left
        {
            get =>  position.X;
            set => position.X = value;
        }

        public float Top
        {
            get => position.Y;
            set => position.Y = value;
        }

        public float Width
        {
            get => size.X;
            set => size.X = value;
        }

        public float Height
        {
            get => size.Y;
            set => size.Y = value;
        }


        public bool Enabled { get => enabled; set => enabled = value; }

        public Control(){
            this.position = new OpenTK.Vector2(100, 100);
            this.size = new OpenTK.Vector2(100, 30);
            this.borderThickness = 1;
            borderRadius = 1;
        }

        public void Draw(OpenTK.Vector2 offset)
        {
            NvgContext vg = ApplicationBase.Instance.MainWindow.Vg;
            if (scissorContents)
                NanoVg.Scissor(vg, position.X, position.Y, size.X,size.Y);
            
            NanoVg.BeginPath(vg);
           
            NanoVg.RoundedRect(vg, position.X, position.Y, size.X, size.Y,borderRadius);
            NanoVg.FillColor(vg,backgroundColor);
            
            NanoVg.Fill(vg);
            NanoVg.FillColor(vg, textColor) ;
            if (borderThickness > 0)
            {
                NanoVg.RoundedRect(vg, position.X, position.Y, size.X, size.Y, borderRadius);
                NanoVg.StrokeColor(vg, borderColor);
                NanoVg.StrokeWidth(vg, borderThickness);
                NanoVg.Stroke(vg);
            }
            NanoVg.FontSize(vg, 18);
            NanoVg.Text(vg, 100, 124, "Hage");
            OnDraw(this.position);
            NanoVg.ResetScissor(vg);
        }
        protected virtual void OnDraw(OpenTK.Vector2 offset) { }
    }
}
