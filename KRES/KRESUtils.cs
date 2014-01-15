using System;
using System.IO;
using System.Reflection;
using System.Linq;
using UnityEngine;

namespace KRES
{
    public static class KRESUtils
    {
        public const double DegToRad = Math.PI / 180d;
        public const double RadToDeg = 180d / Math.PI;
        public static Color BlankColour
        {
            get { return new Color(0, 0, 0, 0); }
        }

        public static bool IsCelestialBody(string name)
        {
            if (FlightGlobals.Bodies.Any(body => body.bodyName == name)) { return true; }
            return false;
        }

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

        public static string GetSavePath()
        {
            return Path.Combine(KSPUtil.ApplicationRootPath, "saves/" + HighLogic.fetch.GameSaveFolder);
        }

        public static string ColorToString(Color value)
        {
            return value.r + ", " + value.g + ", " + value.b + ", " + value.a;
        }

        /// <summary>
        /// Get a Color from vector string.  Returns white if there was a problem.
        /// </summary>
        public static Color StringToColor(string vectorString)
        {
            string[] splitValue = vectorString.Split(',');

            if (splitValue.Length == 3)
            {
                float r, g, b;

                if (!float.TryParse(splitValue[0].Trim(), out r)) return Color.white;
                if (!float.TryParse(splitValue[1].Trim(), out g)) return Color.white;
                if (!float.TryParse(splitValue[2].Trim(), out b)) return Color.white;

                return new Color(r, g, b);
            }
            else if (splitValue.Length == 4)
            {
                float r, g, b, a;

                if (!float.TryParse(splitValue[0].Trim(), out r)) return Color.white;
                if (!float.TryParse(splitValue[1].Trim(), out g)) return Color.white;
                if (!float.TryParse(splitValue[2].Trim(), out b)) return Color.white;
                if (!float.TryParse(splitValue[3].Trim(), out a)) return Color.white;

                return new Color(r, g, b, a);
            }
            return Color.white;
        }

        /// <summary>
        /// Parse colour from a vector string and place it in ref variable.  Leaves ref variable untouched if there was a problem.
        /// </summary>
        public static bool TryStringToColor(string vectorString, ref Color value)
        {
            string[] splitValue = vectorString.Split(',');

            if (splitValue.Length == 3)
            {
                float r, g, b;

                if (!float.TryParse(splitValue[0].Trim(), out r)) return false;
                if (!float.TryParse(splitValue[1].Trim(), out g)) return false;
                if (!float.TryParse(splitValue[2].Trim(), out b)) return false;

                value = new Color(r, g, b);
                return true;
            }
            else if (splitValue.Length == 4)
            {
                float r, g, b, a;

                if (!float.TryParse(splitValue[0].Trim(), out r)) return false;
                if (!float.TryParse(splitValue[1].Trim(), out g)) return false;
                if (!float.TryParse(splitValue[2].Trim(), out b)) return false;
                if (!float.TryParse(splitValue[3].Trim(), out a)) return false;

                value = new Color(r, g, b, a);
                return true;
            }
            return false;
        }
    }
}
