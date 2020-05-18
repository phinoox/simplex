using Simplex.Core.Gui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using System.Windows.Markup;
using System.Xml;

namespace Simplex.Core.Loaders
{
    public class GuiLoader
    {

        public void LoadGui(string filePath)
        {
            Control ctrl = new Control()
            {
                Left = 100.0f,
                Top = 100.0f,
                Width=200,
                Height=40,

            };
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                string savedButton = XamlWriter.Save(ctrl);
                writer.Write(savedButton);
            }
            // Load the button
            StringReader stringReader = new StringReader(filePath);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            Control readerLoadButton = (Control)XamlReader.Load(xmlReader);
        }
    }
}
