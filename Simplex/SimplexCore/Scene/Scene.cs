﻿using Simplex.Core.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simplex.Core.Scene
{
    public class Scene3D
    {
        SceneNode rootNode = new SceneNode();
        Camera currentCamera;
        Renderer sceneRenderer;
    }
}
