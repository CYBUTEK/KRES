using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace KRES
{
    public static class KRESUtils
    {
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
