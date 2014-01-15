using System.Collections.Generic;
using UnityEngine;

namespace KRES.Defaults
{
    public class DefaultLibrary
    {
        #region Instance
        private static DefaultLibrary instance;
        public static DefaultLibrary Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DefaultLibrary();
                }
                return instance;
            }
        }
        #endregion

        #region Properties
        private Dictionary<string, DefaultConfig> defaults = new Dictionary<string, DefaultConfig>();
        public Dictionary<string, DefaultConfig> Defaults
        {
            get { return this.defaults; }
        }

        private DefaultConfig selectedDefault = new DefaultConfig();
        public DefaultConfig SelectedDefault
        {
            get { return this.selectedDefault; }
            set { this.selectedDefault = value; }
        }
        #endregion

        #region Initialisation
        private DefaultLibrary()
        {
            Load();
        }
        #endregion

        #region Public Methods
        public void Load()
        {
            this.defaults.Clear();

            foreach (UrlDir.UrlConfig urlConfig in GameDatabase.Instance.GetConfigs("KRES_DEFAULTS"))
            {
                DefaultConfig defaultConfig = new DefaultConfig(urlConfig.config);
                this.defaults.Add(defaultConfig.Name, defaultConfig);

                if (defaultConfig.Name == "Default")
                {
                    this.selectedDefault = defaultConfig;
                }
            }
        }
        #endregion

        #region Static Methods
        public static List<DefaultConfig> GetDefaults()
        {
            return new List<DefaultConfig>(Instance.Defaults.Values);
        }

        public static List<string> GetDefaultNames()
        {
            return new List<string>(Instance.Defaults.Keys);
        }

        public static DefaultConfig GetDefault(string name)
        {
            if (instance.Defaults.ContainsKey(name))
            {
                return Instance.Defaults[name];
            }
            return null;
        }

        public static DefaultConfig GetSelectedDefault()
        {
            return Instance.SelectedDefault;
        }

        public static void SetSelectedDefault(DefaultConfig defaultConfig)
        {
            Instance.SelectedDefault = defaultConfig;
        }

        private static void CreateResource(ConfigNode cfg, string name, Color colour, double density, string type, double octaves, double persistence, double frequency)
        {
            cfg.AddNode("KRES_RESOURCE").AddValue("name", name);
            foreach (ConfigNode resource in cfg.GetNodes("KRES_RESOURCE"))
            {
                if (resource.HasValue("name") && resource.GetValue("name") == name && !resource.HasValue("type"))
                {
                    if (type == "ore") { resource.AddValue("seed", UnityEngine.Random.Range(0, 999999999).ToString("000000000")); }
                    resource.AddValue("colour", colour.r.ToString() + ", " + colour.g + ", " + colour.b.ToString() + ", " + colour.a.ToString());
                    resource.AddValue("density", density);
                    resource.AddValue("type", type);
                    resource.AddValue("octaves", octaves);
                    resource.AddValue("persistence", persistence);
                    resource.AddValue("frequency", frequency);
                }
            }
        }

        private static void CreateResource(ConfigNode cfg, string name, Color colour, double density, string type, double octaves, double persistence, double frequency, string biome)
        {
            cfg.AddNode("KRES_RESOURCE").AddValue("name", name);
            foreach (ConfigNode resource in cfg.GetNodes("KRES_RESOURCE"))
            {
                if (resource.HasValue("name") && resource.GetValue("name") == name && !resource.HasValue("type"))
                {
                    if (type == "ore") { resource.AddValue("seed", UnityEngine.Random.Range(0, 999999999).ToString("000000000")); }
                    resource.AddValue("colour", colour.r.ToString() + ", " + colour.g + ", " + colour.b.ToString() + ", " + colour.a.ToString());
                    resource.AddValue("density", density);
                    resource.AddValue("type", type);
                    resource.AddValue("octaves", octaves);
                    resource.AddValue("persistence", persistence);
                    resource.AddValue("frequency", frequency);
                    resource.AddValue("biome", biome);
                }
            }
        }

        public static void SaveSelectedDefault(string path)
        {
            ConfigNode configNode = new ConfigNode("KRES");
            configNode.AddNode(Instance.SelectedDefault.CreateConfigNode());
            configNode.Save(path);
        }

        public static void SaveDefaults(ConfigNode config, string path)
        {
            DefaultConfig defaults = GetSelectedDefault();
            ConfigNode cfg = new ConfigNode("KRES");

            if (!cfg.HasValue("name")) { cfg.AddValue("name", defaults.Name); }
            if (!cfg.HasValue("generated")) { cfg.AddValue("generated", false); }

            foreach (CelestialBody body in FlightGlobals.Bodies)
            {
                if (body.name != "Sun" && !cfg.HasNode(body.name))
                {
                    //Create body node               
                    Debug.Log("[KRES]: Creating " + body.bodyName + " node");
                    cfg.AddNode(body.bodyName);
                    ConfigNode node = cfg.GetNode(body.bodyName);
                    if (defaults.HasBody(body.bodyName))
                    {
                        foreach (DefaultResource resource in defaults.GetBody(body.bodyName).Resources)
                        {
                            if (resource.Biome.Length > 0) { CreateResource(node, resource.Name, resource.Colour, resource.Density, resource.Type, resource.Octaves, resource.Persistence, resource.Frequency, resource.Biome); }
                            else { CreateResource(node, resource.Name, resource.Colour, resource.Density, resource.Type, resource.Octaves, resource.Persistence, resource.Frequency); }
                        }
                    }
                    else { Debug.LogWarning("[KRES]: The " + defaults.Name + " defaults file does not contain a definition for " + body.bodyName); }
                }
            }

            config.AddNode(cfg);
            config.Save(path);
        }

        public static bool HasComponents(ConfigNode cfg)
        {
            foreach (CelestialBody body in FlightGlobals.Bodies)
            {
                if (!cfg.nodes.Contains(body.bodyName)) { return false; }
            }
            if (!cfg.HasValue("name") || !cfg.HasValue("generated")) { return false; }
            return true;
        }
        #endregion
    }
}
