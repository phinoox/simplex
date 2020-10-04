using System;
using Simplex.Gui;

namespace SXMLCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            SXMLCreator creator = new SXMLCreator();
            creator.CreateXSD(typeof(Simplex.Gui.Dock),"Simplex.Gui");
        }
    }
}
