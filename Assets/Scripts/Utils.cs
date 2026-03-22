
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace DefaultNamespace
{
    public static class Utils
    {
        public static Vector2 ToXY(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }
        
        /// <summary>
        /// Vector2を回転させる
        /// </summary>
        /// <param name="from">回転させたいVector2</param>
        /// <param name="angle">回転させる角度</param>
        /// <returns></returns>
        public static Vector2 Rotate(this Vector2 from, float angle)
        {
            Complex a = new Complex(from.x, from.y);
            float cos = Mathf.Cos(Mathf.Deg2Rad * angle);
            float sin = Mathf.Sin(Mathf.Deg2Rad * angle);
            Complex b = new Complex(cos, sin);
            Complex result = a * b;
            return new Vector2((float) result.Real, (float) result.Imaginary);
        }

    }
}