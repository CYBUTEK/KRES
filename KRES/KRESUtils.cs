using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KRES
{
    public enum ResourceType
    {
        ORE,
        LIQUID,
        GAS
    }

    public static class KRESUtils
    {
        #region Constants
        public const double DegToRad = Math.PI / 180d;
        public const double RadToDeg = 180d / Math.PI;
        public static readonly Color BlankColour = new Color(0, 0, 0, 0);
        public static readonly Texture2D BlankTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        public static readonly Dictionary<ResourceType, string> types = new Dictionary<ResourceType, string>(3)
        {
            { ResourceType.ORE, "ore" },
            { ResourceType.LIQUID, "liquid" },
            { ResourceType.GAS, "gas" }
        };
        #endregion

        public static string DataURL
        {
            get { return Path.Combine(GetSavePath(), "KRESData.cfg"); }
        }

        public static bool IsCelestialBody(string name)
        {
            return FlightGlobals.Bodies.Any(body => body.bodyName == name);
        }

        public static GUIStyle BoldLabel
        {
            get
            {
                GUIStyle style = new GUIStyle(HighLogic.Skin.label);
                style.fontStyle = FontStyle.Bold;
                return style;
            }
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
            double r = cartesian.magnitude;
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

        public static string GetKRESVersion()
        {
            System.Version version = Assembly.GetExecutingAssembly().GetName().Version;
            if (version.Revision == 0)
            {
                if (version.Build == 0)
                {
                    return version.ToString(2);
                }
                return version.ToString(3);
            }
            return version.ToString();
        }

        public static string ColourToString(Color value)
        {
            return value.r + ", " + value.g + ", " + value.b + ", " + value.a;
        }

        public static string[] ParseArray(string text)
        {
            return text.Split(',').Select(s => s.Trim()).ToArray();
        }

        /// <summary>
        /// Get a Color from vector string.  Returns white if there was a problem.
        /// </summary>
        public static Color StringToColour(string vectorString)
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

        public static ResourceType GetResourceType(string name)
        {
            return types.First(pair => pair.Value == name).Key;
        }

        public static string GetTypeString(ResourceType type)
        {
            return types.First(pair => pair.Key == type).Value;
        }

        public static string SecondsToTime(double seconds)
        {
            string result = string.Empty;
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            if (time.Days > 0) { result += time.TotalDays.ToString().Split('.')[0] + "d "; }
            if (time.Hours > 0) { result += time.Hours + "h "; }
            if (time.Minutes > 0) { result += time.Minutes + "m "; }
            if (time.Seconds > 0) { result += time.Seconds + "s"; }
            return result.Trim();
        }

        public static List<CelestialBody> GetRelevantBodies(string type)
        {
            switch (type)
            {
                case "ore":
                    return new List<CelestialBody>(FlightGlobals.Bodies.Where(b => b.pqsController != null));

                case "gas":
                    return new List<CelestialBody>(FlightGlobals.Bodies.Where(b => b.atmosphere));

                case "liquid":
                    return new List<CelestialBody>(FlightGlobals.Bodies.Where(b => b.ocean));

                default:
                    return new List<CelestialBody>();
            }
        }

        public static GUIStyle GetLabelOfColour(string name)
        {
            Color colour = ResourceInfoLibrary.Instance.GetResource(name).Colour;
            GUIStyle style = new GUIStyle(HighLogic.Skin.label);
            style.normal.textColor = colour;
            style.hover.textColor = colour;
            style.fontStyle = FontStyle.Bold;
            return style;
        }

        public static double Clamp01(double value)
        {
            if (value > 1) { return -value + 2d; }
            if (value < 0) { return -value; }
            return value;
        }
    }
}
