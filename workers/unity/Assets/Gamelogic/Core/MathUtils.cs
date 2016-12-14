using Improbable.Math;
using UnityEngine;

namespace Assets.Gamelogic.Core
{
    public static class MathUtils
    {
        public static Coordinates ToCoordinates(this Vector3 vector3)
        {
            return new Coordinates(vector3.x, vector3.y, vector3.z);
        }

        public static Vector3 ToVector3(this Coordinates coordinates)
        {
            return new Vector3((float)coordinates.X, (float)coordinates.Y, (float)coordinates.Z);
        }

        public static Vector3 FlattenVector(this Vector3 vector3)
        {
            return new Vector3(vector3.x, 0f, vector3.z);
        }

        public static bool CompareEqualityEpsilon(float a, float b)
        {
            return Mathf.Abs(a - b) < Mathf.Epsilon;
        }

        public static bool CompareEqualityEpsilon(Vector3 a, Vector3 b)
        {
            return (a - b).sqrMagnitude < Mathf.Epsilon;
        }

        public static bool CompareEqualityEpsilon(Coordinates a, Coordinates b)
        {
            return Coordinates.SquareDistance(a, b) < Mathf.Epsilon;
        }
    }
}
