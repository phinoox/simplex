using Portable.Xaml;
using Simplex.Core.Gui;
using System.IO;
using System.Xml;

namespace Simplex.Core.Loaders
{
    /// <summary>
    /// class for loading gui from xaml files
    /// </summary>
    public class GuiLoader
    {
        #region Public Methods

        /// <summary>
        /// loads a guiwindow from a xaml file
        /// </summary>
        /// <param name="filePath">the path to the file</param>
        public void LoadGui(string filePath)
        {
            GuiRenderer.DefaultRenderer.Windows.Clear();
            GuiWindow gwin = new GuiWindow();

            // Load the window
            StringReader stringReader = new StringReader(filePath);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            XamlObjectWriterSettings writerSettings = new XamlObjectWriterSettings();

            GuiWindow readerLoadButton = (GuiWindow)XamlServices.Load(filePath);
            GuiRenderer.DefaultRenderer.Windows.Add(readerLoadButton);
        }

        #endregion Public Methods
    }
}