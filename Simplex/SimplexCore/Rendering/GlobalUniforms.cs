using System;
using OpenTK;

namespace Simplex.Core.Rendering{

public static class GlobalUniforms{

   public static Vector3 LightDir = new Vector3(-3,4,-3).Normalized();
   public static Color LightColor = new Color(245,205,195,255);

   public static Color AmbientColor = new Color(0,0,0,255);

   public static uint time;

}


}