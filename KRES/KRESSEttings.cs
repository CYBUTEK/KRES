using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KRES
{
    public class KRESSettings
    {
        #region Defaults
        public static ConfigNode[] GetDefaults()
        {
            List<ConfigNode> nodes = new List<ConfigNode>();
            foreach (ConfigNode node in GameDatabase.Instance.GetConfigNodes("KRES_DEFAULTS")) { nodes.Add(node); }
            return nodes.ToArray();
        }

        public static ConfigNode GetDefault(string name)
        {
            return GameDatabase.Instance.GetConfigNodes("KRES_DEFAULTS").First(node => node.GetValue("name") == name);
        }

        #endregion

        #region ConfigCreators
        //Adds a KRES_RESOURCE node
        private static void CreateResource(ConfigNode cfg, string name, string colour, string density, string type, string octaves, string persistence, string frequency)
        {
            cfg.AddNode("KRES_RESOURCE").AddValue("name", name);
            foreach (ConfigNode resource in cfg.GetNodes("KRES_RESOURCE"))
            {
                if (resource.HasValue("name") && resource.GetValue("name") == name && !resource.HasValue("type"))
                {
                    if (type == "ore") { resource.AddValue("seed", UnityEngine.Random.Range(0, 999999999).ToString("000000000")); }
                    resource.AddValue("colour", colour);
                    resource.AddValue("density", density);
                    resource.AddValue("type", type);
                    resource.AddValue("octaves", octaves);
                    resource.AddValue("persistence", persistence);
                    resource.AddValue("frequency", frequency);
                }
            }
        }

        private static void CreateResource(ConfigNode cfg, string name, string colour, string density, string type, string octaves, string persistence, string frequency, string biome)
        {
            cfg.AddNode("KRES_RESOURCE").AddValue("name", name);
            foreach (ConfigNode resource in cfg.GetNodes("KRES_RESOURCE"))
            {
                if (resource.HasValue("name") && resource.GetValue("name") == name && !resource.HasValue("type"))
                {
                    if (type == "ore") { resource.AddValue("seed", UnityEngine.Random.Range(0, 999999999).ToString("000000000")); }
                    resource.AddValue("colour", colour);
                    resource.AddValue("density", density);
                    resource.AddValue("type", type);
                    resource.AddValue("octaves", octaves);
                    resource.AddValue("persistence", persistence);
                    resource.AddValue("frequency", frequency);
                    resource.AddValue("biome", biome);
                }
            }
        }

        //Checks if any component is missing
        public static bool HasComponents(ConfigNode cfg)
        {
            bool planets = true;
            foreach (CelestialBody body in FlightGlobals.Bodies)
            {
                if (!cfg.HasNode(body.bodyName))
                {
                    planets = false;
                    break;
                }
            }

            if (!planets) { return false; }

            else if (!cfg.HasValue("name") || !cfg.HasValue("octaves") || !cfg.HasValue("persistence") || !cfg.HasValue("frequency")) { return false; }

            else { return true; }
        }

        #endregion

        public static void GenerateSettings(ConfigNode cfg, string def)
        {
            ConfigNode defaults = GetDefault(def);
            //Checks for all planet nodes
            foreach (CelestialBody body in FlightGlobals.Bodies)
            {
                if (!cfg.HasNode(body.bodyName) && body.bodyName != "Sun")
                {
                    //Create body node               
                    Debug.Log("[KRES]: Creating " + body.bodyName + " node");
                    cfg.AddNode(body.bodyName);
                    ConfigNode node = cfg.GetNode(body.bodyName);
                    if (defaults.GetNode(body.bodyName) != null)
                    {
                        foreach (ConfigNode resource in defaults.GetNode(body.bodyName).GetNodes("KRES_RESOURCE"))
                        {
                            string name = string.Empty, colour = string.Empty, density = string.Empty, type = string.Empty, biome = string.Empty;
                            string octaves = string.Empty, persistence = string.Empty, frequency = string.Empty;
                            if (resource.HasValue("name")) { name = resource.GetValue("name"); }
                            if (resource.HasValue("colour")) { colour = resource.GetValue("colour"); }
                            if (resource.HasValue("density")) { density = resource.GetValue("density"); }
                            if (resource.HasValue("type")) { type = resource.GetValue("type"); }
                            if (resource.HasValue("biome")) { biome = resource.GetValue("biome"); }
                            if (resource.HasValue("octaves")) { octaves = resource.GetValue("octaves"); }
                            if (resource.HasValue("persistence")) { persistence = resource.GetValue("persistence"); }
                            if (resource.HasValue("frequency")) { frequency = resource.GetValue("frequency"); }

                            if (biome.Length > 0) { CreateResource(node, name, colour, density, type, octaves, persistence, frequency, biome); }
                            else { CreateResource(node, name, colour, density, type, octaves, persistence, frequency); }
                        }
                    }

                    else { Debug.LogWarning("[KRES]: The " + def + " defaults file does not contain a definition for" + body.bodyName); }
                }
            }
        }
    }
}
