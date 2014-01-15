using UnityEngine;
using System.Linq;

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
        public static bool TryGetValue(this ConfigNode node, string name, ref int value)
        {
            if (node.HasValue(name))
            {
                int result = value;

                if (int.TryParse(node.GetValue(name), out result))
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
                if (KRESUtils.TryStringToColor(node.GetValue(name), ref value))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get a value and place it into the ref variable and return true. Otherwise returns false and leaves the ref variable untouched.
        /// </summary>
        public static bool TryGetValue(this ConfigNode node, string name, ref bool value)
        {
            if (node.HasValue(name))
            {
                bool result = value;

                if (bool.TryParse(node.GetValue(name), out result))
                {
                    value = result;
                    return true;
                }
                DebugWindow.Log(name + " was not parsable.");
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
        public static bool TryGetValue(this ConfigNode node, string name, out int value, int defaultValue)
        {
            if (node.HasValue(name))
            {
                if (int.TryParse(node.GetValue(name), out value))
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
                if (KRESUtils.TryStringToColor(node.GetValue(name), ref defaultValue))
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

        /// <summary>
        /// Get a value and place it into the ref variable and return true. Otherwise returns false and populates the ref variable with the defaultValue.
        /// </summary>
        public static bool TryGetValue(this ConfigNode node, string name, out bool value, bool defaultValue)
        {
            if (node.HasValue(name))
            {
                if (bool.TryParse(node.GetValue(name), out value))
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
        #endregion

        #region TryAddValue
        /// <summary>
        /// Checks if a value exists for a node, if not, it adds one.
        /// </summary>
        public static void TryAddValue(this ConfigNode node, string name, string value)
        {
            if (!node.HasValue(name))
            {
                node.AddValue(name, value);
            }
        }

        /// <summary>
        /// Checks if a value exists for a node, if not, it adds one.
        /// </summary>
        public static void TryAddValue(this ConfigNode node, string name, object value)
        {
            if (!node.HasValue(name))
            {
                node.AddValue(name, value);
            }
        }

        /// <summary>
        /// Checks if a value exists for a node, if not, it adds one.
        /// </summary>
        public static void TryAddValue(this ConfigNode node, string name, Color value)
        {
            if (!node.HasValue(name))
            {
                node.AddValue(name, KRESUtils.ColorToString(value));
            }
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

        #region HasValues
        /// <summary>
        /// Finds out if the node has all the listed values.
        /// </summary>
        public static bool HasValues(this ConfigNode node, string value1, string value2)
        {
            if (node.HasValue(value1) && node.HasValue(value2)) { return true; }
            return false;
        }

        /// <summary>
        /// Finds out if the node has all the listed values.
        /// </summary>
        public static bool HasValues(this ConfigNode node, string value1, string value2, string value3)
        {
            if (node.HasValues(value1, value2) && node.HasValue(value3)) { return true; }
            return false;
        }

        /// <summary>
        /// Finds out if the node has all the listed values.
        /// </summary>
        public static bool HasValues(this ConfigNode node, string value1, string value2, string value3, string value4)
        {
            if (node.HasValues(value1, value2, value3) && node.HasValue(value4)) { return true; }
            return false;
        }

        /// <summary>
        /// Finds out if the node has all the listed values.
        /// </summary>
        public static bool HasValues(this ConfigNode node, string[] values)
        {
            if (values.All(v => node.HasValue(v))) { return true; }
            return false;
        }
        #endregion
    }
}
