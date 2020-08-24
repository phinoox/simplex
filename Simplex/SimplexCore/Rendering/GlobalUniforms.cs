using System;
using OpenTK;

namespace Simplex.Core.Rendering{

public static class GlobalUniforms{

   public static Matrix4 View = Matrix4.Identity;
   public static Matrix4 Projection = Matrix4.Identity;

   public static Vector3 LightDir = new Vector3(2,-4,-2).Normalized();
   public static Color LightColor = new Color(225,205,175,255);

   public static Color AmbientColor = new Color(0,0,0,255);

   public static uint time;

}


}