using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simplex.Core
{
    public static class WorldDefaults
    {
        public static readonly Vector3 Up = Vector3.UnitY;
        public static readonly Vector3 Forward = Vector3.UnitZ *-1;
        public static readonly Vector3 Right = Vector3.UnitX;
    }
}
