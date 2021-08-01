using System;
using OpenTK;
using OpenTK.Mathematics;
using Simplex.Core.Util;

namespace Simplex.Core.Rendering{

public static class GlobalUniforms{

   public static Matrix4 View = Matrix4.Identity;
   public static Matrix4 Projection = Matrix4.Identity;
   public static Matrix4 LightSpaceMatrix = Matrix4.Identity;

   public static Vector3 LightDir = new Vector3(2,-4,-2).Normalized();
   public static SimplexColor LightColor = new SimplexColor(225,205,175,255);

   public static SimplexColor AmbientColor = new SimplexColor(20,20,20,255);
   public static int ShadowMap =0;

   public static uint time;

}


}