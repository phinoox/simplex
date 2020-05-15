using System;
using System.Collections.Generic;
using System.Text;

namespace Simplex.Core.Scene
{
    public enum CameraTypes
    {
        Orthographic,
        Perspective
    }

    public class Camera : SceneNode
    {
        CameraTypes cameraType;
    }
}
