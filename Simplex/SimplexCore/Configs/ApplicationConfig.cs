using System;
using System.Collections.Generic;
using System.Text;

namespace Simplex.Core.Configs
{
    public sealed class ApplicationConfig
    {
        string DataDirectory = "Data";
        string title = "Simplex Application";
        float fps = 60;

        public string DataDirectory1 { get => DataDirectory; set => DataDirectory = value; }
        public string Title { get => title; set => title = value; }
        public float Fps { get => fps; set => fps = value; }
    }
}
