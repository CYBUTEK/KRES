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
        private static void CreateResource(ConfigNode cfg, string name, float density, string type)
        {
            cfg.AddNode("KRES_RESOURCE").AddValue("name", name);
            foreach (ConfigNode resource in cfg.GetNodes("KRES_RESOURCE"))
            {
                if (resource.HasValue("name") && resource.GetValue("name") == name && !resource.HasValue("type"))
                {
                    if (type == "ore") { resource.AddValue("seed", UnityEngine.Random.Range(0, 999999999).ToString("000000000")); }
                    resource.AddValue("density", density);
                    resource.AddValue("type", type);
                }
            }
        }

        private static void CreateResource(ConfigNode cfg, string name, float density, string type, string biome)
        {
            cfg.AddNode("KRES_RESOURCE").AddValue("name", name);
            foreach (ConfigNode resource in cfg.GetNodes("KRES_RESOURCE"))
            {
                if (resource.HasValue("name") && resource.GetValue("name") == name && !resource.HasValue("type"))
                {
                    if (type == "ore") { resource.AddValue("seed", UnityEngine.Random.Range(0, 999999999).ToString("000000000")); }
                    resource.AddValue("density", density);
                    resource.AddValue("type", type);
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
                if (!cfg.HasNode(body.bodyName))
                {
                    //Create body node               
                    Debug.Log("[KRES]: Creating " + body.bodyName + " node");
                    cfg.AddNode(body.bodyName);
                    ConfigNode node = cfg.GetNode(body.bodyName);
                    if (defaults.GetNode(body.bodyName) != null)
                    {
                        foreach (ConfigNode resource in defaults.GetNode(body.bodyName).GetNodes("KRES_RESOURCES"))
                        {
                            string name = string.Empty;
                            float density = 0;
                            string type = string.Empty;
                            string biome = string.Empty;
                            if (resource.HasValue("name")) { name = resource.GetValue("name"); }
                            if (resource.HasValue("density"))
                            {
                                float dens;
                                if (float.TryParse(resource.GetValue("density"), out dens)) { density = dens; }
                            }
                            if (resource.HasValue("type")) { type = resource.GetValue("type"); }
                            if (resource.HasValue("biome")) { biome = resource.GetValue("biome"); }

                            if (biome.Length > 0) { CreateResource(cfg, name, density, type, biome); }
                            else { CreateResource(cfg, name, density, type); }
                        }
                    }

                    else { Debug.LogWarning("[KRES]: The " + def + " defaults file does not contain a definition for" + body.bodyName); }
                }
            }
        }
    }
}
