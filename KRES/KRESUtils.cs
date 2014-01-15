using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace KRES
{
    public static class KRESUtils
    {
        public const double DegToRad = Math.PI / 180d;
        public const double RadToDeg = 180d / Math.PI;

        public static bool TryParseCelestialBody(string name, out CelestialBody result)
        {
            foreach (CelestialBody body in FlightGlobals.Bodies)
            {
                if (body.bodyName == name)
                {
                    result = body;
                    return true;
                }
            }
            result = null;
            return false;
        }

        #region Spherical/cartesian
        public static Vector3d CartesianToSpherical(Vector3d cartesian)
        {
            double r = Math.Sqrt(Math.Pow(cartesian.x, 2d) + Math.Pow(cartesian.y, 2d) + Math.Pow(cartesian.z, 2d));
            double theta = Math.Atan(cartesian.y / cartesian.x) * RadToDeg;
            double phi = Math.Acos(cartesian.z / r) * RadToDeg;
            return new Vector3d(r, theta, phi);
        }

        public static Vector3d CartesianToSpherical(double x, double y, double z)
        {
            double r = Math.Sqrt(Math.Pow(x, 2d) + Math.Pow(y, 2d) + Math.Pow(z, 2d));
            double theta = Math.Atan(y / x) * RadToDeg;
            double phi = Math.Acos(z / r) * RadToDeg;
            return new Vector3d(r, theta, phi);
        }

        public static Vector3d SphericalToCartesian(Vector3d spherical)
        {
            double x = spherical.x * Math.Sin(spherical.z * DegToRad) * Math.Cos(spherical.y * DegToRad);
            double y = spherical.x * Math.Sin(spherical.z * DegToRad) * Math.Sin(spherical.y * DegToRad);
            double z = spherical.x * Math.Cos(spherical.z * DegToRad);
            return new Vector3d(x, y, z);
        }

        public static Vector3d SphericalToCartesian(double r, double theta, double phi)
        {
            double x = r * Math.Sin(phi * DegToRad) * Math.Cos(theta * DegToRad);
            double y = r * Math.Sin(phi * DegToRad) * Math.Sin(theta * DegToRad);
            double z = r * Math.Cos(phi * DegToRad);
            return new Vector3d(x, y, z);
        }

        #endregion

        public static GUIStyle GetDottyFontStyle()
        {
            ScreenMessages messages = (GameObject.FindObjectOfType(typeof(ScreenMessages)) as ScreenMessages);

            foreach (GUIStyle style in messages.textStyles)
            {
                if (style.font.name == "dotty")
                {
                    return style;
                }
            }
            return HighLogic.Skin.label;
        }

        public static string GetDLLPath()
        {
            return new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
        }
    }
}
