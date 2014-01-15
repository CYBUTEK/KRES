using UnityEngine;

namespace KRES.Extensions
{
    public static class ConfigNodeExt
    {
        #region TryGetValue
        /// <summary>
        /// Get a value and place it into the ref variable and return true. Otherwise returns false and leaves the ref variable untouched.
        /// </summary>
        public static bool TryGetValue(this ConfigNode node, string name, ref string value)
        {
            if (node.HasValue(name))
            {
                value = node.GetValue(name);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get a value and place it into the ref variable and return true. Otherwise returns false and leaves the ref variable untouched.
        /// </summary>
        public static bool TryGetValue(this ConfigNode node, string name, ref float value)
        {
            if (node.HasValue(name))
            {
                float result = value;

                if (float.TryParse(node.GetValue(name), out result))
                {
                    value = result;
                    return true;
                }
                DebugWindow.Log(name + " was not parsable.");
            }
            return false;
        }

        /// <summary>
        /// Get a value and place it into the ref variable and return true. Otherwise returns false and leaves the ref variable untouched.
        /// </summary>
        public static bool TryGetValue(this ConfigNode node, string name, ref double value)
        {
            if (node.HasValue(name))
            {
                double result = value;

                if (double.TryParse(node.GetValue(name), out result))
                {
                    value = result;
                    return true;
                }
                DebugWindow.Log(name + " was not parsable.");
            }
            return false;
        }

        /// <summary>
        /// Get a value and place it into the ref variable and return true. Otherwise returns false and leaves the ref variable untouched.
        /// </summary>
        public static bool TryGetValue(this ConfigNode node, string name, ref Color value)
        {
            if (node.HasValue(name))
            {
                if (TryParseColor(node.GetValue(name), ref value))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get a value and place it into the ref variable and return true. Otherwise returns false and populates the ref variable with the defaultValue.
        /// </summary>
        public static bool TryGetValue(this ConfigNode node, string name, out string value, string defaultValue)
        {
            if (node.HasValue(name))
            {
                value = node.GetValue(name);
                return true;
            }
            value = defaultValue;
            return false;
        }

        /// <summary>
        /// Get a value and place it into the ref variable and return true. Otherwise returns false and populates the ref variable with the defaultValue.
        /// </summary>
        public static bool TryGetValue(this ConfigNode node, string name, out float value, float defaultValue)
        {
            if (node.HasValue(name))
            {
                if (float.TryParse(node.GetValue(name), out value))
                {
                    return true;
                }
                DebugWindow.Log(name + " was not parsable.");
            }
            value = defaultValue;
            return false;
        }

        /// <summary>
        /// Get a value and place it into the ref variable and return true. Otherwise returns false and populates the ref variable with the defaultValue.
        /// </summary>
        public static bool TryGetValue(this ConfigNode node, string name, out double value, double defaultValue)
        {
            if (node.HasValue(name))
            {
                if (double.TryParse(node.GetValue(name), out value))
                {
                    return true;
                }
                else
                {
                    DebugWindow.Log(name + " was not parsable.");
                    value = defaultValue;
                    return false;
                }
            }
            value = defaultValue;
            return false;
        }

        /// <summary>
        /// Get a value and place it into the ref variable and return true. Otherwise returns false and populates the ref variable with the defaultValue.
        /// </summary>
        public static bool TryGetValue(this ConfigNode node, string name, out Color value, Color defaultValue)
        {
            if (node.HasValue(name))
            {
                if (TryParseColor(node.GetValue(name), ref defaultValue))
                {
                    value = defaultValue;
                    return true;
                }
                else
                {
                    DebugWindow.Log(name + " was not parsable.");
                    value = defaultValue;
                    return false;
                }
            }
            value = defaultValue;
            return false;
        }
        #endregion

        #region GetValue
        /// <summary>
        /// Get a value if available, otherwise returns the defaultValue.
        /// </summary>
        public static string GetValue(this ConfigNode node, string name, string defaultValue)
        {
            if (node.HasValue(name))
            {
                return node.GetValue(name);
            }
            return defaultValue;
        }
        #endregion

        #region GetAddValue
        /// <summary>
        /// Gets a value if available, otherwise adds the value and then returns the defaultValue
        /// </summary>
        public static string GetAddValue(this ConfigNode node, string name, string defaultValue)
        {
            if (node.HasValue(name))
            {
                return node.GetValue(name);
            }
            node.AddValue(name, defaultValue);
            return defaultValue;
        }
        #endregion

        #region TryParseColor
        /// <summary>
        /// Parse vectorString and place in ref variable and return true.  Otherwise returns false and leaves the ref variable untouched. 
        /// </summary>
        public static bool TryParseColor(string vectorString, ref Color value)
        {
            Color result = ConfigNode.ParseColor(vectorString);

            if (result != null)
            {
                value = result;
                return true;
            }
            return false;
        }
        #endregion
    }
}
