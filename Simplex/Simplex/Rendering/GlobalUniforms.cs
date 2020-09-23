using System;
using System.Numerics;
using Simplex.Util;

namespace Simplex.Rendering{

public static class GlobalUniforms{

   public static Matrix4x4 View = Matrix4x4.Identity;
   public static Matrix4x4 Projection = Matrix4x4.Identity;

   public static Vector3 LightDir = Vector3.Normalize(new Vector3(2,-4,-2));
   public static SimplexColor LightColor = new SimplexColor(225,205,175,255);

   public static Vector4 AmbientColor = new Vector4(0,0,0,255);

   public static uint time;

}


}