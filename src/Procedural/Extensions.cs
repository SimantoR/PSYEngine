using System.Linq;
using System.Numerics;
using UVector3 = UnityEngine.Vector3;
using UVector2 = UnityEngine.Vector2;

namespace PSYEngine.Procedural.Unity
{
    public static class Extensions
    {
        public static UVector3 ToUnityVector(this Vector3 vector)
        {
            return new UVector3(vector.X, vector.Y, vector.Z);
        }

        public static UVector3[] ToUnityVectors(this Vector3[] vectors)
        {
            return vectors.Cast<UVector3>().ToArray();
        }

        public static Vector3[] ToSystemVectors(this UVector3[] vectors)
        {
            return vectors.Cast<Vector3>().ToArray();
        }

        public static Vector2[] ToSystemVectors(this UVector2[] vectors)
        {
            return vectors.Cast<Vector2>().ToArray();
        }
    }
}
