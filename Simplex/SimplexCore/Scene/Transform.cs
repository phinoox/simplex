using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace Simplex.Core.Scene
{
    public class Transformation
    {
        Vector3 translation = Vector3.Zero;
        Quaternion rotation = Quaternion.Identity;
        Vector3 scale = new Vector3(1);

    }
}
