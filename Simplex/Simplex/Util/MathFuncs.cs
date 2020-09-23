using System;
using System.Numerics;

namespace Simplex.Util
{

    public static class MathFuncs
    {
        public static Quaternion RotationBetweenVectors(Vector3 start, Vector3 dest)
        {
            start = Vector3.Normalize(start);
            dest = Vector3.Normalize(dest);

            float cosTheta = Vector3.Dot(start, dest);
            Vector3 rotationAxis;

            if (cosTheta < -1 + 0.001f)
            {
                // special case when vectors in opposite directions:
                // there is no "ideal" rotation axis
                // So guess one; any will do as long as it's perpendicular to start
                rotationAxis = Vector3.Cross(Vector3.UnitZ, start);
                if (rotationAxis.Length() < 0.01) // bad luck, they were parallel, try again!
                    rotationAxis = Vector3.Cross(Vector3.UnitX, start);

                 rotationAxis = Vector3.Normalize(rotationAxis);
                return Quaternion.CreateFromAxisAngle(rotationAxis, OpenTK.MathHelper.DegreesToRadians(180.0f));
            }

            rotationAxis = Vector3.Cross(start, dest);

            float s = MathF.Sqrt((1 + cosTheta) * 2);
            float invs = 1 / s;

            return new Quaternion(
                rotationAxis.X * invs,
                rotationAxis.Y * invs,
                rotationAxis.Z * invs,
              s * 0.5f
            );

        }

        public static Quaternion Lookat(Vector3 position, Vector3 target)
        {
            Vector3 destination = target - position;
            Vector3 forward = Vector3.UnitZ;// * -1f;
            Vector3 desiredUp = Vector3.UnitY;
            // Find the rotation between the front of the object (that we assume towards +Z,
            // but this depends on your model) and the desired direction
            Quaternion rot1 = RotationBetweenVectors(forward, destination);
            // Recompute desiredUp so that it's perpendicular to the direction
            // You can skip that part if you really want to force desiredUp
            Vector3 right = Vector3.Cross(destination, desiredUp);
            desiredUp = Vector3.Cross(right, destination);

            // Because of the 1rst rotation, the up is probably completely screwed up.
            // Find the rotation between the "up" of the rotated object, and the desired up
            Vector3 newUp = Vector3.Transform(Vector3.UnitY,rot1);
            Quaternion rot2 = RotationBetweenVectors(newUp, desiredUp);
            Quaternion targetOrientation = rot2 * rot1; // remember, in reverse order.
            return targetOrientation;
        }
    }

}